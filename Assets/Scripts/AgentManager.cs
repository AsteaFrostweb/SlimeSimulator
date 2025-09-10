using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;

public class AgentManager : MonoBehaviour
{
    struct Agent
    {
        public Vector2 position;
        public float angle;
        public float speed;
        public float lifetime;
        public int alive;
        Vector2 padding;
    }

    [SerializeField] private Material displayMaterial;
    [SerializeField] private ComputeShader slimeShader;

    [SerializeField] private int agentCount = 10000;
    [SerializeField] private Vector2 textureSize = new Vector2(512, 512);
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private Color trailColour = Color.white;
    [Range(0f, 1f)]
    [SerializeField] private float trailPassiveReduction = 0.0002f;
    [SerializeField] private float trailPassiveDiffusion = 0.001f;
    [SerializeField] private float diffuseTrailCuttoffMagnitude = 0.05f;
    [SerializeField] private float sensorDistance = 5;
    [SerializeField] private float sensorAngle = 30;
    [SerializeField] private float turnSpeed = 30;
    [SerializeField] private float brightnessLifetimeScalar = 1f;
    [SerializeField] private float wallAvoidanceStrength = 20f;
    [SerializeField] private bool Bounce = false;
    [SerializeField] private float agentLifetime;

    private RenderTexture trailTexture;
    private ComputeBuffer agentBuffer;
    private ComputeBuffer deadAgents;
    private ComputeBuffer deadAgentCount;
    private Agent[] agents;

    private int agentKernel;
    private int diffuseKernel;
    private Toggle bounceToggle;
    private int slimeCoutnOnReset; // used to make sure same number of threads are dispatched even in agentcount changes in UI
    [SerializeField]private ColorPicker AgentColourUI;
    [SerializeField] private Toggle centerSpawnToggle;
    private void Start()
    {
       ResetSimulation();
    }

    private Vector2 GetAgentSpawnPosition()
    {
        if (centerSpawnToggle.isOn) 
        {
            return new Vector2(Mathf.FloorToInt(textureSize.x / 2), Mathf.FloorToInt(textureSize.y / 2));
        }
        return new Vector2(Range(0, textureSize.x), Range(0, textureSize.y));
    }
    private void InitializeAgents()
    {
        agents = new Agent[agentCount];
        for (int i = 0; i < agentCount; i++)
        {
            agents[i].position = GetAgentSpawnPosition();

            agents[i].angle = Range(0, Mathf.PI * 2);
            agents[i].speed = Range(minSpeed, maxSpeed);
            agents[i].lifetime = agentLifetime;
            agents[i].alive = 1;
        }

        agentBuffer?.Release();
        deadAgents?.Release();
        deadAgentCount?.Release();

        agentBuffer = new ComputeBuffer(agentCount, 32);
        agentBuffer.SetData(agents);

        deadAgents = new ComputeBuffer(agentCount, sizeof(uint));
        deadAgents.SetData(new uint[agentCount]);

        deadAgentCount = new ComputeBuffer(1, sizeof(uint));
        deadAgentCount.SetData(new uint[] { 0 });
    }

    private void InitializeRenderTexture()
    {
        trailTexture?.Release();

        trailTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
        trailTexture.enableRandomWrite = true;
        trailTexture.Create();

        if (displayMaterial != null)
            displayMaterial.mainTexture = trailTexture;
    }

    private void SetupShader()
    {
        if (slimeShader == null) return;

        agentKernel = slimeShader.FindKernel("CSMain");
        diffuseKernel = slimeShader.FindKernel("DiffuseKernel");

        slimeShader.SetBuffer(agentKernel, "agents", agentBuffer);
        slimeShader.SetBuffer(agentKernel, "deadAgents", deadAgents);
        slimeShader.SetBuffer(agentKernel, "deadAgentCount", deadAgentCount);
        slimeShader.SetTexture(agentKernel, "trailMap", trailTexture);
        slimeShader.SetInt("agentCount", agentCount);
        slimeShader.SetTexture(diffuseKernel, "trailMap", trailTexture);

        slimeCoutnOnReset = agentCount;
        UpdateShaderParams();
    }

    private void InitializeUI()
    {
        bounceToggle = GameObject.Find("Toggle(Bounce)")?.GetComponent<Toggle>();
        SetUIValues();
    }

    public void ResetSimulation()
    {
        InitializeAgents();
        InitializeRenderTexture();
        SetupShader();
        SetUIValues();
    }

    private void Update()
    {
        GetUIvalues();
        UpdateShaderParams();

        int csMainKernel = slimeShader.FindKernel("CSMain");
        int diffuseKernel = slimeShader.FindKernel("DiffuseKernel");

        slimeShader.Dispatch(csMainKernel, Mathf.CeilToInt(slimeCoutnOnReset / 1024f), 1, 1);
        slimeShader.Dispatch(diffuseKernel, Mathf.CeilToInt((trailTexture.width * trailTexture.height) / 1024f), 1, 1);
    }

    private void OnDestroy()
    {
        agentBuffer?.Release();
        deadAgents?.Release();
        deadAgentCount?.Release();
        trailTexture?.Release();
    }

    private void SetUIValues()
    {
        if (bounceToggle != null)
            bounceToggle.isOn = Bounce;

        AgentColourUI.SetColor(trailColour, 0, 1);

        SettingsOptionsManager.SetValue("Agent Count", agentCount);
        SettingsOptionsManager.SetValue("Texture Size", (int)textureSize.x);
        SettingsOptionsManager.SetValue("Min Speed", minSpeed);
        SettingsOptionsManager.SetValue("Max Speed", maxSpeed);
        SettingsOptionsManager.SetValue("Trail Reduction", trailPassiveReduction);
        SettingsOptionsManager.SetValue("Trail Diffusion", trailPassiveDiffusion);
        SettingsOptionsManager.SetValue("Diffuse Cutoff", diffuseTrailCuttoffMagnitude);
        SettingsOptionsManager.SetValue("Sensor Distance", sensorDistance);
        SettingsOptionsManager.SetValue("Sensor Angle", sensorAngle);
        SettingsOptionsManager.SetValue("Turn Speed", turnSpeed);
        SettingsOptionsManager.SetValue("BLS", brightnessLifetimeScalar);
        SettingsOptionsManager.SetValue("Lifetime", agentLifetime);
    }

    private void GetUIvalues()
    {
        if (bounceToggle != null)
            Bounce = bounceToggle.isOn;

        agentCount = SettingsOptionsManager.GetValue<int>("Agent Count");

        int texSize = SettingsOptionsManager.GetValue<int>("Texture Size");
        textureSize = new Vector2(texSize, texSize);
        trailColour = AgentColourUI.GetColor(0f, 1f);
        minSpeed = SettingsOptionsManager.GetValue<float>("Min Speed");
        maxSpeed = SettingsOptionsManager.GetValue<float>("Max Speed");
        trailPassiveReduction = SettingsOptionsManager.GetValue<float>("Trail Reduction");
        trailPassiveDiffusion = SettingsOptionsManager.GetValue<float>("Trail Diffusion");
        diffuseTrailCuttoffMagnitude = SettingsOptionsManager.GetValue<float>("Diffuse Cutoff");
        sensorDistance = SettingsOptionsManager.GetValue<float>("Sensor Distance");
        sensorAngle = SettingsOptionsManager.GetValue<float>("Sensor Angle");
        turnSpeed = SettingsOptionsManager.GetValue<float>("Turn Speed");
        brightnessLifetimeScalar = SettingsOptionsManager.GetValue<float>("BLS");
        agentLifetime = SettingsOptionsManager.GetValue<float>("Lifetime");
    }

    private void UpdateShaderParams()
    {
        slimeShader.SetVector("TRAIL_INTENSITY", trailColour);
        slimeShader.SetBool("bounce", Bounce);
        slimeShader.SetFloat("wallAvoidanceStrength", wallAvoidanceStrength);
        slimeShader.SetFloat("turnSpeed", turnSpeed);
        slimeShader.SetFloat("brightnessLifetimeScalar", brightnessLifetimeScalar);
        slimeShader.SetFloat("sensorDistance", sensorDistance);
        slimeShader.SetFloat("sensorAngle", sensorAngle);
        slimeShader.SetFloat("deltaTime", Time.deltaTime);
        slimeShader.SetFloat("threshold", diffuseTrailCuttoffMagnitude);
        slimeShader.SetFloat("PASSIVE_TRAIL_DIFFUSION_FACTOR", trailPassiveDiffusion);
        slimeShader.SetFloat("PASSIVE_TRAIL_RECUCTION", trailPassiveReduction);
        slimeShader.SetVector("TRAIL_INTENSITY", trailColour);
    }


    
}
