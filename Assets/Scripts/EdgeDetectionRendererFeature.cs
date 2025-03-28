using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetectionRendererFeature : ScriptableRendererFeature
{
    public class EdgeDetectionPass : ScriptableRenderPass
    {
        #region 设置变量

        private float edgeOnly = 1.0f; //设置shader
        public Color edgeColor;//1, 1, 1, 1);
        public Color _BackgroundColor;
        
        static readonly string RenderEventPassName = "CustomRenderPassEvent"; //定义一个静态的字符创名称，用于显示在FrameDebug的渲染事件中
        static readonly int MainTexID = Shader.PropertyToID("_MainTex"); //shader的_MainTex变量，作为主纹理贴图给ID
        static readonly int tempTexID = Shader.PropertyToID("_TempEdgeDetectionRT"); //声明临时贴图的ID

        Material m_material; //新建材质接口
        // EdgeDetectionVolume volume; //新建Volume接口

        RenderTargetIdentifier source; //定义当前相机目标

        #endregion

        //Pass初始化，设置渲染事件、判断调用的shader、创建材质
        public EdgeDetectionPass(RenderPassEvent renderEvent, Settings set)
        {
            renderPassEvent = renderEvent; //设置渲染事件位置，这里的renderPassEvent是从ScriptableRenderPass里面得到的
            var shader = set.shader; //通过shader创建材质，便于后面通过材质参数进行计算
            if (shader == null)
            {
                return;
            }

            _BackgroundColor = set._BackgroundColor;
            edgeColor = set._edgeColor;
            edgeOnly = set._edgeOnly;

            m_material = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_material == null)
            {
                Debug.LogError("材质初始化创建失败!");
            }

            if (!renderingData.cameraData.postProcessEnabled)
            {
                Debug.LogError("相机的后处理未激活！");
                return;
            }

            // var stack = VolumeManager.instance.stack; //Volume相关，创建堆栈，
            // volume = stack.GetComponent<EdgeDetectionVolume>(); //从堆栈中获取到相应的Volume组件
            // if (volume == null)
            // {
            //     Debug.LogError("Volume组件获取失败!");
            //     return;
            // }

            CommandBuffer cmd = CommandBufferPool.Get(RenderEventPassName); //实现在FrameDebug里面渲染事件的位置

            Render(cmd, ref renderingData);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        //ref作用是函数内部参数改变，对应外部参数也跟着改变
        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTargetHandle; //当前相机

            RenderTextureDescriptor tempDescriptor = renderingData.cameraData.cameraTargetDescriptor; //声明临时RT
            int rtWidth = tempDescriptor.width; //定义临时RT的宽度
            int rtHeight = tempDescriptor.height; //定义临时RT的高度

            //将Volume里面的值传递到shader里面对应的属性
            m_material.SetFloat("_EdgeOnly", edgeOnly);
            m_material.SetColor("_EdgeColor", edgeColor);
            m_material.SetColor("_BackgroundColor", _BackgroundColor);

            cmd.SetGlobalTexture(MainTexID, source); //给MainTexID赋值，将source的渲染结果传递给材质
            cmd.GetTemporaryRT(tempTexID, rtWidth, rtHeight, depthBuffer: 0, FilterMode.Trilinear,
                format: RenderTextureFormat.Default);

            //开始绘制计算
            cmd.Blit(source, tempTexID,m_material,0);
            cmd.Blit(tempTexID, source);

            cmd.ReleaseTemporaryRT(tempTexID); //将计算完成之后释放申请的临时RT
        }
    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;         //设置渲染事件执行位置，在后处理之前
        public Shader shader;
        public float _edgeOnly = 1.0f; //设置shader
        public Color _edgeColor;//1, 1, 1, 1);
        public Color _BackgroundColor;
    }
    // [SerializeField]
    public Settings settings = new Settings();                     //class类里面定义的方法，需要再外面在创建出来
    
    EdgeDetectionPass m_ScriptablePass;                     //声明EdgeDetection脚本，定义渲染Pass

    public override void Create()
    {
        this.name = "EdgeDetection";                        //这里定义的name是在RenderFeature上面显示的名字，并不是FrameDebug面板的事件名称
        m_ScriptablePass = new EdgeDetectionPass(settings.renderPassEvent , settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);     //将该Pass添加到渲染队列中
    }

    // EdgeDetectionRenderPass m_ScriptablePass;
    //
    // /// <inheritdoc/>
    // public override void Create()
    // {
    //     m_ScriptablePass = new EdgeDetectionRenderPass();
    //
    //     // Configures where the render pass should be injected.
    //     m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    // }
    //
    // // Here you can inject one or multiple render passes in the renderer.
    // // This method is called when setting up the renderer once per-camera.
    // public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    // {
    //     renderer.EnqueuePass(m_ScriptablePass);
    // }
}


