using UnityEngine;

public class BillboardParticles : MonoBehaviour
{
    public Shader geomShader;

    public Texture2D square;
    public Texture2D circle;
    public Texture2D cross;
    public Vector2 size = Vector2.one;
    public Color color = new Color(1.0f, 0.6f, 0.3f, 0.03f);

    ComputeBuffer outputBuffer;

    [Range(0, 2)]
    [Tooltip("Billboard type 0 = static, 1 = Cylindrical, 2 = Sphecrical")]
    public int billboardType = 2;

    private PointData[] points;
    private Material material;

    struct PointData
    {
        public Vector3 pos;
        public Color color;
        public int shape;
    };

    // Use this for initialization
    void Start()
    {
        material = new Material(geomShader);

        // Data sampled from https://syntagmatic.github.io/parallel-coordinates/examples/data/cars.csv
        var cars = new float[,]
        {
            // year, weight (lbs), power (hp), economy (mpg), num cylinders
            {73f, 3821f, 175f, 13f, 8f},
            {70f, 3850f, 190f, 15f, 8f},
            {72f, 3672f, 150f, 17f, 8f},
            {79f, 3265f, 90f, 20.2f, 6f},
            {78f, 3410f, 120f, 18.1f, 6f},
            {78f, 3210f, 90f, 19.4f, 6f},
            {80f, 3003f, 90f, 24.3f, 4f},
            {73f, 2789f, 100f, 18f, 6f},
            {71f, 2634f, 100f, 19f, 6f},
            {75f, 2914f, 100f, 20f, 6f},
            {70f, 2648f, 90f, 21f, 6f},
            {71f, 2962f, 110f, 18f, 6f},
            {70f, 2774f, 97f, 18f, 6f},
            {73f, 2945f, 100f, 18f, 6f},
            {74f, 2901f, 100f, 19f, 6f},
            {76f, 3085f, 90f, 22.5f, 6f},
            {74f, 4257f, 150f, 14f, 8f},
            {72f, 3892f, 150f, 15f, 8f},
            {73f, 3672f, 150f, 14f, 8f},
            {75f, 3730f, 110f, 15f, 6f},
            {76f, 3962f, 120f, 15.5f, 8f},
            {74f, 3632f, 110f, 16f, 6f},
            {71f, 3288f, 100f, 18f, 6f},
            {76f, 3193f, 95f, 17.5f, 6f},
            {75f, 3211f, 90f, 19f, 6f},
            {70f, 3433f, 150f, 16f, 8f},
            {79f, 2670f, 80f, 27.4f, 4f},
            {73f, 2582f, 91f, 20f, 4f},
            {75f, 2694f, 95f, 23f, 4f},
            {70f, 2430f, 90f, 24f, 4f},
            {80f, 2188f, 78f, 34.3f, 4f},
            {74f, 2219f, 83f, 29f, 4f},
            {70f, 2234f, 113f, 26f, 4f},
            {77f, 2600f, 110f, 21.5f, 4f},
            {73f, 4100f, 175f, 13f, 8f},
            {82f, 2945f, 110f, 25f, 6f},
            {74f, 4699f, 150f, 13f, 8f},
            {78f, 3380f, 105f, 20.6f, 6f},
            {75f, 3907f, 110f, 17f, 6f},
            {81f, 3415f, 110f, 22.4f, 6f},
            {73f, 4951f, 225f, 12f, 8f},
            {70f, 3086f, 225f, 14f, 8f},
            {79f, 4360f, 155f, 16.9f, 8f},
            {72f, 4502f, 155f, 13f, 8f},
            {77f, 2155f, 80f, 30f, 4f},
            {78f, 3445f, 165f, 17.7f, 6f},
            {75f, 3039f, 110f, 21f, 6f},
            {70f, 3693f, 165f, 15f, 8f},
            {79f, 2670f, 90f, 28.4f, 4f},
            {77f, 3425f, 105f, 20.5f, 6f},
            {81f, 2635f, 84f, 26.6f, 4f},
            {79f, 3900f, 125f, 23f, 8f},
            {76f, 4380f, 180f, 16.5f, 8f},
            {75f, 3897f, 105f, 16f, 6f},
            {75f, 4440f, 145f, 15f, 8f},
            {82f, 2950f, 90f, 27f, 4f},
            {73f, 4464f, 150f, 13f, 8f},
            {79f, 3840f, 130f, 17f, 8f},
            {77f, 3880f, 145f, 17.5f, 8f},
            {82f, 2395f, 88f, 34f, 4f},
            {82f, 2640f, 88f, 27f, 4f},
            {82f, 2605f, 88f, 28f, 4f},
            {72f, 4098f, 130f, 13f, 8f},
            {74f, 3781f, 100f, 16f, 6f},
            {76f, 4215f, 140f, 17.5f, 8f},
            {71f, 3329f, 100f, 17f, 6f},
            {70f, 3504f, 130f, 18f, 8f},
            {76f, 2035f, 52f, 29f, 4f},
            {78f, 2155f, 68f, 30f, 4f},
            {77f, 2051f, 63f, 30.5f, 4f},
            {80f, 2120f, 70f, 32.1f, 4f},
            {81f, 2725f, 110f, 23.5f, 6f},
            {80f, 2678f, 90f, 28f, 4f},
            {79f, 2595f, 115f, 28.8f, 6f},
            {77f, 3520f, 110f, 17.5f, 6f},
            {73f, 4997f, 150f, 11f, 8f},
            {72f, 4274f, 165f, 13f, 8f},
            {71f, 4209f, 165f, 14f, 8f},
            {70f, 4354f, 220f, 14f, 8f},
            {79f, 3605f, 125f, 19.2f, 8f},
            {73f, 3988f, 145f, 13f, 8f},
            {78f, 3155f, 95f, 20.5f, 6f},
            {77f, 4165f, 170f, 15.5f, 8f},
            {78f, 3425f, 145f, 19.2f, 8f},
            {73f, 4082f, 145f, 15f, 8f},
            {70f, 3761f, 150f, 15f, 8f},
            {75f, 3221f, 110f, 20f, 8f},
            {73f, 3278f, 100f, 16f, 6f},
            {74f, 3336f, 100f, 15f, 6f},
            {75f, 3459f, 105f, 18f, 6f},
            {76f, 3353f, 105f, 22f, 6f},
            {71f, 2408f, 72f, 22f, 4f},
            {71f, 2264f, 90f, 28f, 4f},
            {72f, 2408f, 90f, 20f, 4f},
            {73f, 2401f, 72f, 21f, 4f},
            {74f, 2542f, 75f, 25f, 4f},
            {76f, 2164f, 60f, 24.5f, 4f},
            {76f, 4055f, 145f, 13f, 8f},
            {70f, 4376f, 200f, 10f, 8f},
            {82f, 2720f, 82f, 31f, 4f},
            {77f, 4325f, 190f, 15.5f, 8f},
            {82f, 2585f, 92f, 26f, 4f},
            {81f, 3465f, 85f, 17.6f, 6f},
            {79f, 3940f, 150f, 18.5f, 8f},
            {73f, 4735f, 215f, 13f, 8f},
            {72f, 4422f, 190f, 13f, 8f},
            {71f, 1613f, 69f, 35f, 4f},
            {78f, 2405f, 97f, 23.9f, 4f},
            {81f, 2615f, 100f, 32.9f, 4f},
            {79f, 2020f, 65f, 31.8f, 4f},
            {81f, 1975f, 65f, 37f, 4f},
            {80f, 2110f, 65f, 40.8f, 4f},
            {80f, 2910f, 132f, 32.7f, 6f},
            {82f, 1995f, 67f, 38f, 4f},
            {80f, 2019f, 65f, 37.2f, 4f},
            {72f, 2288f, 92f, 28f, 4f},
            {80f, 2434f, 92f, 37f, 4f},
            {78f, 2300f, 97f, 27.2f, 4f},
            {73f, 2379f, 94f, 22f, 4f},
            {75f, 2545f, 97f, 24f, 4f},
            {74f, 2003f, 61f, 32f, 4f},
            {81f, 2930f, 120f, 24.2f, 6f},
            {77f, 2815f, 97f, 22f, 6f},
            {76f, 1990f, 70f, 32f, 4f},
            {78f, 2070f, 70f, 39.4f, 4f},
            {74f, 1950f, 67f, 31f, 4f},
            {77f, 1945f, 70f, 33.5f, 4f},
            {70f, 2130f, 88f, 27f, 4f},
            {71f, 2130f, 88f, 27f, 4f},
            {82f, 2525f, 84f, 29f, 4f},
            {81f, 2620f, 92f, 25.8f, 4f},
            {79f, 3360f, 110f, 20.6f, 6f},
            {76f, 3651f, 100f, 20f, 6f},
            {78f, 3620f, 110f, 18.6f, 6f},
            {80f, 3381f, 90f, 19.1f, 6f},
            {70f, 3563f, 170f, 15f, 8f},
            {82f, 2370f, 84f, 36f, 4f},
            {72f, 2164f, 80f, 28f, 4f},
            {72f, 2126f, 80f, 25f, 4f},
            {79f, 1915f, 80f, 35.7f, 4f},
            {77f, 2075f, 83f, 33.5f, 4f},
            {76f, 2255f, 79f, 26f, 4f},
            {80f, 2800f, 105f, 27.9f, 4f},
            {74f, 2125f, 75f, 28f, 4f},
            {76f, 4190f, 150f, 16f, 8f},
            {74f, 4457f, 150f, 14f, 8f},
            {73f, 3777f, 150f, 15f, 8f},
            {76f, 3755f, 150f, 13f, 8f},
            {70f, 4382f, 210f, 11f, 8f},
            {73f, 3399f, 150f, 15f, 8f},
            {78f, 3735f, 140f, 19.4f, 8f},
            {78f, 4080f, 140f, 17.5f, 8f},
            {71f, 4955f, 180f, 12f, 8f},
            {77f, 4140f, 145f, 15.5f, 8f},
            {78f, 2230f, 75f, 30.9f, 4f},
            {82f, 2295f, 84f, 32f, 4f},
            {79f, 3830f, 135f, 18.2f, 8f},
            {73f, 2265f, 90f, 26f, 4f},
            {74f, 2246f, 75f, 26f, 4f},
            {71f, 2065f, 76f, 30f, 4f},
            {74f, 2108f, 75f, 24f, 4f},
            {73f, 1867f, 49f, 29f, 4f},
            {76f, 2464f, 86f, 28f, 4f},
            {79f, 2130f, 69f, 37.3f, 4f},
            {74f, 2000f, 67f, 31f, 4f},
            {76f, 2572f, 92f, 25f, 4f},
            {71f, 4746f, 170f, 13f, 8f},
            {79f, 4054f, 142f, 15.5f, 8f},
            {73f, 4906f, 167f, 12f, 8f},
            {81f, 2380f, 65f, 29.9f, 4f},
            {81f, 2045f, 65f, 34.4f, 4f},
            {76f, 3870f, 130f, 13f, 8f},
            {70f, 4615f, 215f, 10f, 8f},
            {78f, 2965f, 85f, 20.2f, 6f},
            {78f, 2720f, 88f, 25.1f, 4f},
            {79f, 2890f, 88f, 22.3f, 4f},
            {82f, 2865f, 92f, 24f, 4f},
            {80f, 2870f, 88f, 26.4f, 4f},
            {78f, 1800f, 66f, 36.1f, 4f},
            {78f, 3205f, 139f, 18.1f, 8f},
            {72f, 4129f, 153f, 14f, 8f},
            {71f, 4154f, 153f, 14f, 8f},
            {70f, 4341f, 198f, 15f, 8f},
            {72f, 4294f, 140f, 13f, 8f},
            {74f, 4638f, 140f, 14f, 8f},
            {73f, 4042f, 137f, 14f, 8f},
            {76f, 4215f, 152f, 14.5f, 8f},
            {74f, 4141f, 140f, 16f, 8f},
            {76f, 3574f, 78f, 18f, 6f},
            {81f, 3060f, 88f, 20.2f, 6f},
            {82f, 2835f, 112f, 22f, 6f},
            {77f, 3525f, 98f, 18.5f, 6f},
            {79f, 3725f, 129f, 17.6f, 8f},
            {73f, 4363f, 158f, 13f, 8f},
            {75f, 4657f, 148f, 14f, 8f},
            {75f, 3158f, 72f, 15f, 6f},
            {73f, 3021f, 88f, 18f, 6f},
            {70f, 2587f, 85f, 21f, 6f},
            {76f, 3012f, 81f, 24f, 6f},
            {82f, 2790f, 86f, 27f, 4f},
            {77f, 2755f, 89f, 25.5f, 4f},
            {75f, 3169f, 129f, 13f, 8f},
            {71f, 3139f, 88f, 18f, 6f},
            {72f, 2395f, 86f, 22f, 4f},
            {72f, 2226f, 86f, 21f, 4f},
            {75f, 2984f, 97f, 18f, 6f},
            {73f, 2310f, 85f, 19f, 4f},
            {75f, 2639f, 83f, 23f, 4f},
            {74f, 2451f, 80f, 26f, 4f},
            {76f, 2565f, 72f, 26.5f, 4f},
            {82f, 2625f, 79f, 28f, 4f},
            {77f, 4335f, 149f, 16f, 8f},
            {71f, 3302f, 88f, 19f, 6f},
            {70f, 3449f, 140f, 17f, 8f},
            {70f, 4732f, 193f, 9f, 8f},
            {77f, 2045f, 68f, 31.5f, 4f},
            {78f, 2135f, 68f, 29.5f, 4f},
            {80f, 2290f, 72f, 32.4f, 4f},
            {82f, 2205f, 75f, 36f, 4f},
            {82f, 1965f, 67f, 32f, 4f},
            {81f, 1760f, 60f, 35.1f, 4f},
            {80f, 1850f, 67f, 44.6f, 4f},
            {75f, 1795f, 53f, 33f, 4f},
            {78f, 1800f, 60f, 36.1f, 4f},
            {74f, 2489f, 97f, 24f, 4f},
            {76f, 1795f, 53f, 33f, 4f},
            {82f, 1965f, 67f, 38f, 4f},
            {81f, 2210f, 75f, 33.7f, 4f},
            {79f, 1975f, 65f, 34.1f, 4f},
            {80f, 2542f, 75f, 31.3f, 4f},
            {81f, 2635f, 74f, 31.6f, 4f},
            {81f, 1985f, 68f, 34.1f, 4f},
            {82f, 2025f, 68f, 37f, 4f},
            {82f, 1970f, 68f, 31f, 4f},
            {78f, 1985f, 52f, 32.8f, 4f},
            {80f, 2110f, 65f, 46.6f, 4f},
            {80f, 3250f, 67f, 30f, 4f},
            {76f, 3820f, 120f, 16.5f, 6f},
            {71f, 2220f, 86f, 23f, 4f},
            {73f, 2472f, 107f, 21f, 6f},
            {77f, 4295f, 130f, 15f, 8f},
            {79f, 3955f, 138f, 16.5f, 8f},
            {82f, 2125f, 70f, 36f, 4f},
            {73f, 4952f, 198f, 12f, 8f},
            {72f, 4633f, 208f, 11f, 8f},
            {78f, 3570f, 139f, 20.2f, 8f},
            {75f, 3432f, 72f, 15f, 6f},
            {79f, 2990f, 85f, 19.8f, 6f},
            {78f, 3070f, 85f, 20.8f, 6f},
            {82f, 2160f, 88f, 36f, 4f},
            {82f, 3015f, 85f, 38f, 6f},
            {81f, 3725f, 105f, 26.6f, 8f},
            {78f, 3365f, 110f, 19.9f, 8f},
            {79f, 3420f, 90f, 23.9f, 8f},
            {77f, 4060f, 110f, 17f, 8f},
            {72f, 4456f, 160f, 12f, 8f},
            {79f, 2700f, 115f, 26.8f, 6f},
            {73f, 3664f, 180f, 11f, 8f},
            {78f, 2855f, 85f, 23.8f, 4f},
            {73f, 4499f, 180f, 12f, 8f},
            {76f, 2220f, 81f, 25f, 4f},
            {71f, 2123f, 90f, 28f, 4f},
            {73f, 2158f, 75f, 24f, 4f},
            {74f, 2300f, 78f, 26f, 4f},
            {71f, 2074f, 70f, 30f, 4f},
            {72f, 2979f, 87f, 21f, 4f},
            {76f, 3270f, 88f, 19f, 4f},
            {75f, 2957f, 88f, 23f, 4f},
            {70f, 2672f, 87f, 25f, 4f},
            {79f, 3190f, 71f, 27.2f, 4f},
            {81f, 3230f, 80f, 28.1f, 4f},
            {78f, 3410f, 133f, 16.2f, 6f},
            {77f, 2300f, 96f, 25.5f, 4f},
            {70f, 3609f, 160f, 14f, 8f},
            {81f, 1875f, 64f, 39f, 4f},
            {71f, 1955f, 70f, 26f, 4f},
            {73f, 4654f, 170f, 13f, 8f},
            {74f, 3102f, 95f, 20f, 6f},
            {70f, 2833f, 95f, 22f, 6f},
            {73f, 2904f, 95f, 23f, 6f},
            {73f, 4237f, 150f, 14f, 8f},
            {71f, 4096f, 150f, 14f, 8f},
            {70f, 4312f, 215f, 14f, 8f},
            {72f, 4135f, 150f, 15f, 8f},
            {75f, 3785f, 95f, 18f, 6f},
            {75f, 4498f, 150f, 16f, 8f},
            {81f, 2215f, 63f, 34.7f, 4f},
            {82f, 2125f, 63f, 38f, 4f},
            {79f, 2150f, 70f, 34.5f, 4f},
            {79f, 2200f, 70f, 34.2f, 4f},
            {81f, 2490f, 84f, 27.2f, 4f},
            {81f, 2385f, 84f, 30f, 4f},
            {78f, 2745f, 105f, 23.2f, 4f},
            {72f, 4077f, 150f, 14f, 8f},
            {71f, 3439f, 105f, 16f, 6f},
            {74f, 3613f, 105f, 18f, 6f},
            {70f, 3436f, 150f, 18f, 8f},
            {75f, 3264f, 95f, 19f, 6f},
            {73f, 3121f, 105f, 18f, 6f},
            {76f, 3233f, 100f, 22f, 6f},
            {77f, 3630f, 100f, 19f, 6f},
            {76f, 3940f, 150f, 13f, 8f},
            {78f, 3430f, 100f, 20.5f, 6f},
            {75f, 2592f, 78f, 23f, 4f},
            {71f, 4464f, 175f, 14f, 8f},
            {72f, 4385f, 175f, 14f, 8f},
            {70f, 4425f, 225f, 14f, 8f},
            {75f, 4668f, 170f, 16f, 8f},
            {71f, 3282f, 100f, 19f, 6f},
            {77f, 4220f, 180f, 16f, 8f},
            {73f, 4278f, 230f, 16f, 8f},
            {82f, 2575f, 85f, 31f, 4f},
            {79f, 3245f, 115f, 21.5f, 6f},
            {78f, 3535f, 105f, 19.2f, 6f},
            {82f, 2735f, 90f, 27f, 4f},
            {79f, 2556f, 90f, 33.5f, 4f},
            {71f, 5140f, 175f, 13f, 8f},
            {77f, 2740f, 88f, 24.5f, 4f},
            {76f, 3645f, 110f, 18.5f, 6f},
            {72f, 2189f, 69f, 26f, 4f},
            {76f, 2202f, 83f, 27f, 4f},
            {77f, 1825f, 58f, 36f, 4f},
            {70f, 2375f, 95f, 25f, 4f},
            {78f, 2795f, 115f, 21.6f, 4f},
            {73f, 2660f, 110f, 24f, 4f},
            {75f, 2671f, 115f, 25f, 4f},
            {77f, 1985f, 67f, 30f, 4f},
            {80f, 2145f, 67f, 33.8f, 4f},
            {74f, 2391f, 93f, 26f, 4f},
            {81f, 2065f, 67f, 32.3f, 4f},
            {73f, 2279f, 88f, 20f, 4f},
            {78f, 2515f, 95f, 21.1f, 4f},
            {82f, 2665f, 96f, 32f, 4f},
            {71f, 1773f, 65f, 31f, 4f},
            {74f, 1836f, 65f, 32f, 4f},
            {72f, 2100f, 88f, 27f, 4f},
            {77f, 2265f, 75f, 26f, 4f},
            {80f, 1968f, 60f, 38.1f, 4f},
            {76f, 2155f, 75f, 28f, 4f},
            {75f, 2171f, 75f, 29f, 4f},
            {80f, 2265f, 75f, 32.2f, 4f},
            {81f, 2350f, 75f, 32.4f, 4f},
            {82f, 2245f, 70f, 34f, 4f},
            {72f, 2278f, 95f, 24f, 4f},
            {80f, 2711f, 90f, 29.8f, 4f},
            {70f, 2372f, 95f, 24f, 4f},
            {75f, 2702f, 96f, 24f, 4f},
            {71f, 2228f, 95f, 25f, 4f},
            {78f, 2560f, 95f, 27.5f, 4f},
            {74f, 1649f, 52f, 31f, 4f},
            {81f, 2900f, 116f, 25.4f, 6f},
            {76f, 2930f, 108f, 19f, 6f},
            {73f, 2807f, 122f, 20f, 6f},
            {81f, 1755f, 58f, 39.1f, 4f},
            {81f, 2050f, 62f, 37.7f, 4f},
            {72f, 2506f, 97f, 23f, 4f},
            {80f, 2500f, 88f, 35f, 4f},
            {80f, 1845f, 62f, 29.8f, 4f},
            {70f, 1835f, 46f, 26f, 4f},
            {72f, 2511f, 76f, 22f, 4f},
            {80f, 2335f, 48f, 43.4f, 4f},
            {75f, 2223f, 71f, 25f, 4f},
            {74f, 1963f, 67f, 26f, 4f},
            {77f, 2190f, 78f, 30.5f, 4f},
            {81f, 2190f, 74f, 33f, 4f},
            {71f, 1834f, 60f, 27f, 4f},
            {82f, 2130f, 52f, 44f, 4f},
            {80f, 2085f, 48f, 44.3f, 4f},
            {78f, 1985f, 48f, 43.1f, 4f},
            {77f, 1940f, 78f, 29f, 4f},
            {79f, 1925f, 71f, 31.9f, 4f},
            {82f, 1980f, 74f, 36f, 4f},
            {75f, 1937f, 70f, 29f, 4f},
            {76f, 1937f, 70f, 29f, 4f},
            {76f, 1825f, 71f, 29.5f, 4f},
            {80f, 2144f, 76f, 41.5f, 4f},
            {78f, 1990f, 71f, 31.5f, 4f},
            {73f, 1950f, 46f, 26f, 4f},
            {72f, 2254f, 54f, 23f, 4f},
            {73f, 2868f, 112f, 19f, 4f},
            {72f, 2933f, 112f, 18f, 4f},
            {75f, 2945f, 98f, 22f, 4f},
            {76f, 3150f, 102f, 20f, 4f},
            {78f, 3140f, 125f, 17f, 6f},
            {81f, 3160f, 76f, 30.7f, 6f}
        };

        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        float zMin = float.MaxValue;
        float zMax = float.MinValue;
        float colorMin = float.MaxValue;
        float colorMax = float.MinValue;
        for (int i = 0; i < cars.GetLength(0); i++)
        {
            if (cars[i, 0] < xMin)
            {
                xMin = cars[i, 0];
            }
            if (cars[i, 0] > xMax)
            {
                xMax = cars[i, 0];
            }

            if (cars[i, 1] < yMin)
            {
                yMin = cars[i, 1];
            }
            if (cars[i, 1] > yMax)
            {
                yMax = cars[i, 1];
            }

            if (cars[i, 2] < zMin)
            {
                zMin = cars[i, 2];
            }
            if (cars[i, 2] > zMax)
            {
                zMax = cars[i, 2];
            }

            if (cars[i, 3] < colorMin)
            {
                colorMin = cars[i, 3];
            }
            if (cars[i, 3] > colorMax)
            {
                colorMax = cars[i, 3];
            }
        }

        points = new PointData[cars.GetLength(0)];
        for(int i = 0; i < cars.GetLength(0); i++)
        {
            var x = (cars[i, 0] - xMin) / (xMax - xMin);
            var y = (cars[i, 1] - yMin) / (yMax - yMin);
            var z = (cars[i, 2] - zMin) / (zMax - zMin);
            var h = (cars[i, 3] - colorMin) / (1.1f * (colorMax - colorMin)); // HSV wraps around from red to red, so make it so range isn't fully [0, 1]
            int shape = -1;
            if (cars[i, 4] == 4)
            {
                shape = 0;
            }
            if (cars[i, 4] == 6)
            {
                shape = 1;
            }
            if (cars[i, 4] == 8)
            {
                shape = 2;
            }
            points[i] = new PointData()
            {
                pos = new Vector3(x, y, z),
                color = Color.HSVToRGB(h, 1.0f, 1.0f),//new Color(r, 0.0f, 0.0f, 1.0f),
                shape = shape
            };
        }
        
        outputBuffer = new ComputeBuffer(points.Length, (sizeof(float) * 3) + (sizeof(float) * 4) + sizeof(int));
        outputBuffer.SetData(points);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        material.SetColor("_Color", color);
        material.SetBuffer("geomCenters", outputBuffer);
        material.SetTexture("_SquareTexture", square);
        material.SetTexture("_CircleTexture", circle);
        material.SetTexture("_CrossTexture", cross);
        material.SetVector("_Size", size);
        material.SetInt("_BillboardType", billboardType);
        material.SetVector("_worldPos", transform.position);
        
        Graphics.DrawProcedural(MeshTopology.Points, outputBuffer.count);
    }

    void OnDestroy()
    {
        outputBuffer.Release();
    }
}
