﻿using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using System.IO;

namespace Mapsui.Tests.Common
{
    public static class Utilities
    {
        public static MemoryProvider CreateProviderWithPointsWithVectorStyle()
        {
            var features = new Features
                {
                    new Feature
                        {
                            Geometry = new Point(50, 50),
                            Styles = new[] {new VectorStyle {Fill = new Brush(Color.Red)}}
                        },
                    new Feature
                        {
                            Geometry = new Point(50, 100),
                            Styles = new[] {new VectorStyle {Fill = new Brush(Color.Yellow), Outline = new Pen(Color.Black, 2)}}
                        },
                    new Feature
                        {
                            Geometry = new Point(100, 50),
                            Styles = new[] {new VectorStyle {Fill = new Brush(Color.Blue), Outline = new Pen(Color.White, 2)}}
                        },
                    new Feature
                        {
                            Geometry = new Point(100, 100),
                            Styles = new[] {new VectorStyle {Fill = new Brush(Color.Green), Outline = null}}
                        }
                };
            var provider = new MemoryProvider(features);
            return provider;
        }

        public static MemoryProvider CreateProviderWithPointsWithSymbolStyles()
        {
            const string circleIconPath = @"Mapsui.Tests.Common.Resources.Images.circle.png";
            var circleIcon = typeof(Utilities).Assembly.GetManifestResourceStream(circleIconPath);
            var circleIconId = BitmapRegistry.Instance.Register(circleIcon);
            const string checkeredIconPath = @"Mapsui.Tests.Common.Resources.Images.checkered.png";
            var checkeredIcon = typeof(Utilities).Assembly.GetManifestResourceStream(checkeredIconPath);
            var checkeredIconId = BitmapRegistry.Instance.Register(checkeredIcon);

            var features = new Features
            {
                new Feature
                {
                    Geometry = new Point(50, 50),
                    Styles = new[] { new VectorStyle { Fill = new Brush(Color.Red) }}
                },
                new Feature
                {
                    Geometry = new Point(50, 100),
                    Styles = new[] { new SymbolStyle { BitmapId = circleIconId }}
                },
                new Feature
                {
                    Geometry = new Point(100, 50),
                    Styles = new[] { new SymbolStyle { BitmapId = checkeredIconId }}
                },
                new Feature
                {
                    Geometry = new Point(100, 100),
                    Styles = new[] { new VectorStyle { Fill = new Brush(Color.Green), Outline = null }}
                }
            };
            var provider = new MemoryProvider(features);
            return provider;
        }

        public static MemoryProvider CreatePolygonProvider()
        {
            var provider = new MemoryProvider(new Features());

            var feature = new Feature
                {
                    Geometry = Geometry.GeomFromText(
                            "POLYGON ((-1955226.9536767 4267247.14707544, -2183574.6270988 4267247.14707544, -2611726.51476523 4095986.39200886, -2868617.64736509 4067442.9328311, -3268226.07585376 3896182.17776453, -3667834.50434243 3839095.259409, -4067442.9328311 3639291.04516467, -4495594.82049753 3525117.20845362, -4809572.87145292 3068421.86160943, -5066464.00405278 2954248.02489838, -5266268.21829711 2668813.43312076, -5780050.48349683 2240661.54545433, -6008398.15691892 1612705.44354356, -6265289.28951878 1070379.71916608, -6293832.74869655 813488.586566221, -6408006.58540759 642227.831499647, -6579267.34047417 -299706.321366502, -6779071.5547185 -984749.341632793, -6893245.39142955 -1755422.73943237, -6921788.85060731 -2982791.48407614, -6978875.76896284 -3296769.53503152, -6950332.30978507 -4866659.78980844, -6750528.09554074 -5922767.77938564, -6522180.42211864 -6522180.42211864, -6408006.58540759 -6721984.63636298, -6122571.99362997 -6950332.30978507, -6008398.15691892 -7378484.1974515, -5751507.02431907 -7692462.24840689, -5152094.38158606 -8234787.97278437, -4923746.70816397 -8348961.80949542, -3667834.50434243 -8634396.40127304, -2954248.02489838 -8720026.77880632, -899118.964099508 -8777113.69716185, -156989.025477692 -8834200.61551737, 1213097.01505489 -8834200.61551737, 2697356.89229852 -8777113.69716185, 3810551.80023124 -8634396.40127304, 4552681.73885306 -8291874.89113989, 4838116.33063068 -8120614.13607332, 5209181.29994158 -8006440.29936227, 5351898.5958304 -7920809.92182898, 5551702.81007473 -7635375.33005136, 5780050.48349683 -7521201.49334032, 5922767.77938564 -7292853.81991822, 6094028.53445221 -7150136.52402941, 6322376.20787431 -7035962.68731836, 6607810.79965193 -6779071.5547185, 6721984.63636298 -6522180.42211864, 7064506.14649612 -6065485.07527445, 7093049.60567388 -5894224.32020788, 7178679.98320717 -5694420.10596354, 7178679.98320717 -4923746.70816396, 7093049.60567388 -3981812.55529781, 6978875.76896283 -3496573.74927586, 6921788.85060731 -2925704.56572062, 6921788.85060731 -2611726.51476523, 7007419.2281406 -2497552.67805418, 7007419.2281406 -2440465.75969866, 8291874.89113989 -2155031.16792104, 10118656.2785167 -1898140.03532118, 11288938.1048049 -1555618.52518803, 12116698.42096 -870575.504921744, 12230872.2576711 -613684.372321885, 12373589.5535599 -442423.617255311, 12516306.8494487 -99902.1071221679, 12544850.3086264 642227.831499647, 12544850.3086264 4838116.33063068, 12573393.7678042 5009377.08569725, 12687567.6045153 5380442.05500816, 12915915.2779374 5665876.64678578, 13058632.5738262 5780050.48349683, 13429697.5431371 6293832.74869655, 13515327.9206704 6664897.71800745, 13515327.9206704 7035962.68731836, 13258436.7880705 7464114.57498479, 13030089.1146484 7492658.03416255, 12744654.5228708 7635375.33005136, 12544850.3086264 7692462.24840689, 11060590.4313828 7721005.70758465, 10889329.6763162 7635375.33005136, 10689525.4620719 7492658.03416255, 10575351.6253609 7292853.81991822, 10489721.2478276 7007419.2281406, 10375547.4111165 6864701.93225179, 10289917.0335832 6550723.8812964, 10204286.65605 6408006.58540759, 9947395.5234501 5437528.97336368, 9661960.93167247 4695399.03474187, 9604874.01331695 4381420.98378648, 9148178.66647276 4038899.47365334, 8948374.45222842 3953269.09612005, 8605852.94209528 3724921.42269796, 8206244.5136066 3667834.50434243, 7835179.5442957 3553660.66763138, 7606831.8708736 3553660.66763138, 7407027.65662927 3439486.83092033, 7150136.52402941 3382399.91256481, 6522180.42211864 3325312.99420929, 5951311.2385634 3154052.23914271, 4010356.01447558 3154052.23914271, 3239682.616676 3325312.99420929, 2811530.72900957 3353856.45338705, 2697356.89229852 3410943.37174257, 2212118.08627656 3496573.74927586, 2012313.87203223 3582204.12680915, 1469988.14765475 3639291.04516467, 1327270.85176594 3724921.42269796, 956205.882455029 3753464.88187572, 585140.913144123 3867638.71858677, -528053.994788598 3924725.63694229, -984749.341632795 4067442.9328311, -1070379.71916608 4124529.85118663, -1156010.09669936 4124529.85118663, -1156010.09669936 4153073.31036439, -1612705.44354356 4153073.31036439, -1869596.57614342 4210160.22871991, -1955226.9536767 4267247.14707544, -1955226.9536767 4267247.14707544))")
                };
            feature.Styles.Add(new VectorStyle { Fill = new Brush(Color.Black), Line = new Pen(Color.Black, 3), Outline = new Pen(Color.Orange, 3) });
            provider.Features.Add(feature);

            return provider;
        }

        public static MemoryProvider CreateLineProvider()
        {
            var provider = new MemoryProvider(new Features());

            var feature = new Feature
                {
                    Geometry = Geometry.GeomFromText(
                            "LINESTRING (-642227.831499647 5123550.9224083, -1241640.47423265 5037920.54487501, -1584161.9843658 4923746.70816396, -1869596.57614342 4923746.70816396, -2012313.87203223 4838116.33063068, -2326291.92298761 4809572.87145292, -2583183.05558747 4723942.49391963, -3211139.15749824 4695399.03474187, -3468030.2900981 4552681.73885306, -3610747.58598691 4524138.27967529, -3753464.88187572 4409964.44296425, -3810551.80023124 4409964.44296425, -4124529.85118663 4095986.39200886, -4181616.76954215 4095986.39200886, -4267247.14707544 4010356.01447558, -4467051.36131977 3924725.63694229, -4524138.2796753 3839095.259409, -4923746.70816397 3553660.66763138, -5066464.00405278 3525117.20845362, -5152094.38158606 3439486.83092033, -5237724.75911935 3268226.07585376, -5466072.43254144 3125508.77996495, -5523159.35089697 2982791.48407614, -5780050.48349683 2668813.43312076, -5979854.69774116 2354835.38216537, -6008398.15691892 2240661.54545433, -6065485.07527445 2183574.6270988, -6122571.99362997 1898140.03532118, -6236745.83034102 1755422.73943237, -6265289.28951878 1612705.44354356, -6322376.20787431 842032.045743983, -6322376.20787431 -642227.831499647, -6265289.28951878 -1041836.25998832, -6151115.45280774 -1527075.06601027, -5865680.86103011 -2240661.54545433, -5751507.02431907 -2383378.84134314, -5694420.10596354 -2526096.13723195, -5551702.81007473 -2668813.43312076, -5523159.35089697 -2754443.81065404, -5009377.08569725 -3239682.616676, -4781029.41227515 -3353856.45338705, -4609768.65720858 -3382399.91256481, -4295790.6062532 -3553660.66763138, -3525117.20845362 -3810551.80023124, -3325312.99420929 -3953269.09612005, -2554639.59640971 -4153073.31036439, -2069400.79038775 -4324334.06543096, -984749.341632795 -4524138.27967529, -670771.29067741 -4609768.65720858, -185532.484655455 -4809572.87145291, 271162.862188738 -4895203.2489862, 870575.504921742 -5180637.84076382, 1641248.90272132 -5437528.97336368, 1869596.57614342 -5551702.81007473, 2297748.46380985 -5637333.18760802, 2725900.35147628 -5637333.18760802, 3353856.45338705 -5694420.10596354, 4980833.62651949 -5722963.5651413, 4980833.62651949 -5694420.10596354, 5180637.84076382 -5665876.64678578, 5237724.75911935 -5608789.72843025, 5665876.64678578 -5523159.35089697, 5922767.77938564 -5323355.13665263, 6293832.74869654 -5209181.29994159, 6465093.50376312 -5095007.46323054, 6579267.34047417 -5095007.46323054, 6779071.5547185 -5009377.08569725, 6893245.39142955 -4895203.2489862, 7321397.27909598 -4866659.78980844, 7492658.03416255 -4809572.87145291, 7749549.16676241 -4809572.87145291, 8377505.26867318 -4638312.11638634, 8891287.53387289 -4609768.65720858, 9262352.5031838 -4723942.49391963, 9604874.01331695 -5037920.54487501, 9690504.39085023 -5066464.00405278, 9861765.14591681 -5266268.21829711, 10204286.65605 -5494615.89171921, 10946416.5946718 -5751507.02431907, 11888350.7475379 -5922767.77938564, 12830284.9004041 -5979854.69774116, 14971044.3387362 -5979854.69774116, 15427739.6855804 -5894224.32020788, 15741717.7365358 -5751507.02431907, 15770261.1957136 -5694420.10596354, 15827348.1140691 -5694420.10596354, 16226956.5425578 -5180637.84076382, 16369673.8384466 -4524138.27967529, 16483847.6751576 -4295790.6062532, 16512391.1343354 -4295790.6062532, 16540934.5935131 -4153073.31036439, 16598021.5118687 -4067442.9328311, 16626564.9710464 -3867638.71858677, 16769282.2669352 -3610747.58598691, 16797825.726113 -2811530.72900957, 16854912.6444685 -2611726.51476523, 16854912.6444685 -2383378.84134314, 16911999.5628241 -2240661.54545433, 16940543.0220018 -1726879.28025461, 17026173.3995351 -1555618.52518803, 17140347.2362461 -1469988.14765475, 17197434.1546017 -1327270.85176594, 17654129.5014459 -727858.209032934, 17739759.8789792 -528053.994788598, 17739759.8789792 -356793.239722027)")
                };
            feature.Styles.Add(new VectorStyle { Line = new Pen(Color.Violet, 5) });
            provider.Features.Add(feature);

            return provider;
        }

        public static Feature CreateSimplePointFeature(double x, double y, IStyle style)
        {
            var feature = new Feature { Geometry = new Point(x, y) };
            feature.Styles.Add(style);
            return feature;
        }

        public static IProvider CreateProviderWithRotatedBitmapSymbols()
        {
            var features = new Features
            {
                new Feature { Geometry = new Point(75, 75), Styles = new[] { new SymbolStyle { Fill = new Brush(Color.Red)}}}, // for reference
                CreateFeatureWithRotatedBitmapSymbol(75, 125, 90),
                CreateFeatureWithRotatedBitmapSymbol(125, 125, 180),
                CreateFeatureWithRotatedBitmapSymbol(125, 75, 270)
            };
            return new MemoryProvider(features);
        }

        public static Feature CreateFeatureWithRotatedBitmapSymbol(double x, double y, double rotation)
        {
            const string bitmapPath = @"Mapsui.Tests.Common.Resources.Images.iconthatneedsoffset.png";
            var bitmapStream = typeof(Utilities).Assembly.GetManifestResourceStream(bitmapPath);
            var bitmapId = BitmapRegistry.Instance.Register(bitmapStream);

            var feature = new Feature { Geometry = new Point(x, y) };

            feature.Styles.Add(
                new SymbolStyle
                {
                    BitmapId = bitmapId,
                    SymbolOffset = new Offset { Y = -24 },
                    SymbolRotation = rotation
                });
            return feature;
        }

        public static byte[] ToByteArray(Stream input)
        {
            return ToMemoryStream(input).ToArray();
        }

        public static MemoryStream ToMemoryStream(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream;
            }
        }
    }
}