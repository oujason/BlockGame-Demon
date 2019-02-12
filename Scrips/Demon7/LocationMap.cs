using System.Collections;
using System.Collections.Generic;

namespace LPlan
{
    public class CubeLocationPlan
    {
        //key
        public string key;

        public string plan1;
        public string plan2;
        public string plan3;
        public string plan4;
        public string plan5;
        public string plan6;
        public string plan7;
        public string plan8;
        public string plan9;

        public string plan10;
        public string plan11;
        public string plan12;
        public string plan13;
        public string plan14;
        public string plan15;
        public string plan16;
        public string plan17;
        public string plan18;

        public CubeLocationPlan(string ky,
            string p1 = "", string p2 = "", string p3 = "", string p4 = "", string p5 = "", string p6 = "",
            string p7 = "", string p8 = "", string p9 = "", string p10 = "", string p11 = "", string p12 = "",
            string p13 = "", string p14 = "", string p15 = "", string p16 = "", string p17 = "", string p18 = ""
            )
        {
            key = ky;
            plan1 = p1;
            plan2 = p2;
            plan3 = p3;
            plan4 = p4;
            plan5 = p5;
            plan6 = p6;
            plan7 = p7;
            plan8 = p8;
            plan9 = p9;
            plan10 = p10;
            plan11 = p11;
            plan12 = p12;
            plan13 = p13;
            plan14 = p14;
            plan15 = p15;
            plan16 = p16;
            plan17 = p17;
            plan18 = p18;
        }
    }

    public class LocationMap
    {

        private static LocationMap instance = null;

        public static LocationMap GetInstance
        {            
            get
            {
                if (null == instance)
                {
                    instance = new LocationMap();
                }
                return instance;
            }
        }

        //dict
        Dictionary<string, CubeLocationPlan> locaDict = new Dictionary<string, CubeLocationPlan>();

        public void Initialize()
        {
            //1
            CubeLocationPlan cube1 = new CubeLocationPlan("125", p1: "145", p4: "235", p7: "145", p10: "126", p13: "126", p16: "235");
            //2
            CubeLocationPlan cube2 = new CubeLocationPlan("120", p1: "150", p4: "250", p8: "140", p10: "160", p13: "260", p17: "230");
            //3
            CubeLocationPlan cube3 = new CubeLocationPlan("126", p1: "125", p4: "125", p9: "146", p10: "146", p13: "236", p18: "236");
            //4
            CubeLocationPlan cube4 = new CubeLocationPlan("150", p1: "140", p5: "350", p7: "450", p10: "120", p14: "160", p16: "250");
            //5
            CubeLocationPlan cube5 = new CubeLocationPlan("100", p1: "100", p5: "500", p8: "400", p10: "100", p14: "600", p17: "200");
            //6
            CubeLocationPlan cube6 = new CubeLocationPlan("160", p1: "120", p5: "150", p9: "460", p10: "140", p14: "360", p18: "260");
            //7
            CubeLocationPlan cube7 = new CubeLocationPlan("145", p1: "146", p6: "345", p7: "345", p10: "125", p15: "146", p16: "125");
            //8
            CubeLocationPlan cube8 = new CubeLocationPlan("140", p1: "160", p6: "450", p8: "340", p10: "150", p15: "460", p17: "120");
            //9
            CubeLocationPlan cube9 = new CubeLocationPlan("146", p1: "126", p6: "145", p9: "346", p10: "145", p15: "346", p18: "126");

            //10
            CubeLocationPlan cube10 = new CubeLocationPlan("250", p2: "450", p4: "230", p7: "150", p11: "260", p13: "120", p16: "350");
            //11
            CubeLocationPlan cube11 = new CubeLocationPlan("200", p2: "500", p4: "200", p8: "100", p11: "600", p13: "200", p17: "300");
            //12
            CubeLocationPlan cube12 = new CubeLocationPlan("260", p2: "250", p4: "120", p9: "160", p11: "460", p13: "230", p18: "360");
            //13
            CubeLocationPlan cube13 = new CubeLocationPlan("500", p2: "400", p5: "300", p7: "500", p11: "200", p14: "100", p16: "500");
            //14
            CubeLocationPlan cube14 = new CubeLocationPlan("000", p2: "000", p5: "000", p8: "000", p11: "000", p14: "000", p17: "000");
            //15
            CubeLocationPlan cube15 = new CubeLocationPlan("600", p2: "200", p5: "100", p9: "600", p11: "400", p14: "300", p18: "600");
            //16
            CubeLocationPlan cube16 = new CubeLocationPlan("450", p2: "460", p6: "340", p7: "350", p11: "250", p15: "140", p16: "150");
            //17
            CubeLocationPlan cube17 = new CubeLocationPlan("400", p2: "600", p6: "400", p8: "300", p11: "500", p15: "400", p17: "100");
            //18
            CubeLocationPlan cube18 = new CubeLocationPlan("460", p2: "260", p6: "140", p9: "360", p11: "450", p15: "340", p18: "160");

            //19
            CubeLocationPlan cube19 = new CubeLocationPlan("235", p3: "345", p4: "236", p7: "125", p12: "236", p13: "125", p16: "345");
            //20
            CubeLocationPlan cube20 = new CubeLocationPlan("230", p3: "350", p4: "260", p8: "120", p12: "360", p13: "250", p17: "340");
            //21
            CubeLocationPlan cube21 = new CubeLocationPlan("236", p3: "235", p4: "126", p9: "126", p12: "346", p13: "235", p18: "346");
            //22
            CubeLocationPlan cube22 = new CubeLocationPlan("350", p3: "340", p5: "360", p7: "250", p12: "230", p14: "150", p16: "450");
            //23
            CubeLocationPlan cube23 = new CubeLocationPlan("300", p3: "300", p5: "600", p8: "200", p12: "300", p14: "500", p17: "400");
            //24
            CubeLocationPlan cube24 = new CubeLocationPlan("360", p3: "230", p5: "160", p9: "260", p12: "340", p14: "350", p18: "460");
            //25
            CubeLocationPlan cube25 = new CubeLocationPlan("345", p3: "346", p6: "346", p7: "235", p12: "235", p15: "145", p16: "145");
            //26
            CubeLocationPlan cube26 = new CubeLocationPlan("340", p3: "360", p6: "460", p8: "230", p12: "350", p15: "450", p17: "140");
            //27
            CubeLocationPlan cube27 = new CubeLocationPlan("346", p3: "236", p6: "146", p9: "236", p12: "345", p15: "345", p18: "146");

            locaDict.Add(cube1.key, cube1);
            locaDict.Add(cube2.key, cube2);
            locaDict.Add(cube3.key, cube3);
            locaDict.Add(cube4.key, cube4);
            locaDict.Add(cube5.key, cube5);
            locaDict.Add(cube6.key, cube6);
            locaDict.Add(cube7.key, cube7);
            locaDict.Add(cube8.key, cube8);
            locaDict.Add(cube9.key, cube9);
            locaDict.Add(cube10.key, cube10);
            locaDict.Add(cube11.key, cube11);
            locaDict.Add(cube12.key, cube12);
            locaDict.Add(cube13.key, cube13);
            locaDict.Add(cube14.key, cube14);
            locaDict.Add(cube15.key, cube15);
            locaDict.Add(cube16.key, cube16);
            locaDict.Add(cube17.key, cube17);
            locaDict.Add(cube18.key, cube18);

            locaDict.Add(cube19.key, cube19);
            locaDict.Add(cube20.key, cube20);
            locaDict.Add(cube21.key, cube21);
            locaDict.Add(cube22.key, cube22);
            locaDict.Add(cube23.key, cube23);
            locaDict.Add(cube24.key, cube24);
            locaDict.Add(cube25.key, cube25);
            locaDict.Add(cube26.key, cube26);
            locaDict.Add(cube27.key, cube27);
        }

        public string GetPlan(string key, int plan, bool forward)
        {
            string loca = "999";
            CubeLocationPlan locaPlan = null;
            locaDict.TryGetValue(key, out locaPlan);
            if (locaPlan != null)
            {
                int result = plan;
                if (forward == false)
                {
                    result = 9 + plan;
                }
                switch (result)
                {
                    case 1: loca = locaPlan.plan1; break;
                    case 2: loca = locaPlan.plan2; break;
                    case 3: loca = locaPlan.plan3; break;
                    case 4: loca = locaPlan.plan4; break;
                    case 5: loca = locaPlan.plan5; break;
                    case 6: loca = locaPlan.plan6; break;
                    case 7: loca = locaPlan.plan7; break;
                    case 8: loca = locaPlan.plan8; break;
                    case 9: loca = locaPlan.plan9; break;
                    case 10: loca = locaPlan.plan10; break;
                    case 11: loca = locaPlan.plan11; break;
                    case 12: loca = locaPlan.plan12; break;
                    case 13: loca = locaPlan.plan13; break;
                    case 14: loca = locaPlan.plan14; break;
                    case 15: loca = locaPlan.plan15; break;
                    case 16: loca = locaPlan.plan16; break;
                    case 17: loca = locaPlan.plan17; break;
                    case 18: loca = locaPlan.plan18; break;
                    default: break;
                }
            }        
            return loca;
        }
    }


}
