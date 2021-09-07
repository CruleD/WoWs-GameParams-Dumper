using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Razorvine.Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WoWs_GameParams_Dumper
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            loadGPToolStripMenuItem.Enabled = File.Exists("GameParams.data");
            loadJSONToolStripMenuItem.Enabled = Directory.Exists("JSONDumpFolder");
        }

        private void LoadGPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadGPToolStripMenuItem.Enabled = false;
            new Thread(UnpickleGP).Start();
            menuStrip_Main.Enabled = false;
        }

        private Hashtable UnpickledGP;
        private void UnpickleGP()
        {
            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: Deflating GameParams..."; }));
            byte[] FileBytes = File.ReadAllBytes("GameParams.data").Reverse().ToArray();
            byte[] ModdedFileBytes = new byte[FileBytes.Length - 2]; //remove file header
            Array.Copy(FileBytes, 2, ModdedFileBytes, 0, ModdedFileBytes.Length);

            //using (Stream StreamTemp = new MemoryStream(ModdedFileBytes))
            //{
            //    using (FileStream decompressedFileStream = File.Create("UnpickleDumpTest.txt"))
            //    {
            //        using (DeflateStream DeflateStreamTemp = new DeflateStream(StreamTemp, CompressionMode.Decompress))
            //        {
            //            DeflateStreamTemp.CopyTo(decompressedFileStream);
            //        }
            //    }
            //}

            using (Stream StreamTemp = new MemoryStream(ModdedFileBytes))
            {
                using (MemoryStream MemoryStreamTemp = new MemoryStream())
                {
                    using (DeflateStream DeflateStreamTemp = new DeflateStream(StreamTemp, CompressionMode.Decompress))
                    {
                        DeflateStreamTemp.CopyTo(MemoryStreamTemp);
                    }
                    ModdedFileBytes = MemoryStreamTemp.ToArray();
                }
            }

            Thread.Sleep(1000);
            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: Unpickling GameParams..."; }));
            using (Unpickler UnpicklerTemp = new Unpickler())
            {
                Unpickler.registerConstructor("copy_reg", "_reconstructor", new CustomUnpicklerClass("copy_reg", "_reconstructor"));
                //object[] UnpickledObjectTemp = (object[])UnpicklerTemp.loads(File.ReadAllBytes("UnpickleDumpTest.txt"));
                object[] UnpickledObjectTemp = (object[])UnpicklerTemp.loads(ModdedFileBytes);
                UnpickledGP = (Hashtable)UnpickledObjectTemp[0];
                UnpicklerTemp.close();
            }
            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: GameParams Unpickled!"; }));
            Thread.Sleep(1000);
            label_Status.BeginInvoke(new Action(() =>
            {
                label_Status.Text = "Status: Idle";
                menuStrip_Main.Enabled = true;
                saveJSONToolStripMenuItem.Enabled = true;
            }));
        }

        // - - - - -

        private void SaveJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveJSONToolStripMenuItem.Enabled = false;
            new Thread(WriteGP2JSON).Start();
            menuStrip_Main.Enabled = false;
        }

        private void WriteGP2JSON()
        {
            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: Sorting GameParams..."; }));
            //Sort JSON for human reading
            SortedDictionary<string, SortedDictionary<object, object>> SortedDictionayJSON = new SortedDictionary<string, SortedDictionary<object, object>>();
            foreach (DictionaryEntry UnpickledJSONEntry in UnpickledGP)
            {
                string UnpickledJSONEntry_Key = UnpickledJSONEntry.Key.ToString();
                SortedDictionary<object, object> UnpickledJSONEntry_Value = ConvertDataValue(UnpickledJSONEntry.Value);

                foreach (string ThisKey in UnpickledJSONEntry_Value.Keys.ToList())
                {
                    object ThisValue = UnpickledJSONEntry_Value[ThisKey];

                    if (IsThereSomething2Sort(ref ThisValue)) //collection type
                    {
                        string JSONParam_Key = ThisKey;
                        if (!JSONParam_Key.Equals("damageDistribution")) //some bad data
                        {
                            SortedDictionary<object, object> JSONParam_Value = ConvertDataValue(ThisValue);
                            foreach (object ThisKey0 in JSONParam_Value.Keys.ToList())
                            {
                                object ThisValue0 = JSONParam_Value[ThisKey0];
                                if (IsThereSomething2Sort(ref ThisValue0)) //collection type
                                {
                                    object JSONParam0_Key = ThisKey0;
                                    SortedDictionary<object, object> JSONParam0_Value = ConvertDataValue(ThisValue0);
                                    foreach (object ThisKey1 in JSONParam0_Value.Keys.ToList())
                                    {
                                        object ThisValue1 = JSONParam0_Value[ThisKey1];
                                        if (IsThereSomething2Sort(ref ThisValue1)) //collection type
                                        {
                                            SortedDictionary<object, object> JSONParam1_Value = ConvertDataValue(ThisValue1);
                                            JSONParam0_Value[ThisKey1] = JSONParam1_Value;
                                        }
                                    }
                                    JSONParam_Value[JSONParam0_Key] = JSONParam0_Value;
                                }
                            }
                            UnpickledJSONEntry_Value[JSONParam_Key] = JSONParam_Value;
                        }
                    }
                }
                SortedDictionayJSON.Add(UnpickledJSONEntry_Key, UnpickledJSONEntry_Value);
            }

            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: Writing GameParams to JSON..."; }));
            JsonSerializer JSONSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            foreach (KeyValuePair<string, SortedDictionary<object, object>> TempValueHolder in SortedDictionayJSON)
            {
                SortedDictionary<object, object> KeyJSONData = TempValueHolder.Value;
                string TypeString = ((SortedDictionary<object, object>)KeyJSONData["typeinfo"])["type"].ToString();
                string FolderPath = string.Format("JSONDumpFolder\\{0}", TypeString);
                Directory.CreateDirectory(FolderPath);
                using (StreamWriter StreamWriterTemp = new StreamWriter(string.Format("{0}\\{1}.json", FolderPath, TempValueHolder.Key.ToString())))
                {
                    using (JsonWriter JsonWriterTemp = new JsonTextWriter(StreamWriterTemp))
                    {
                        JsonWriterTemp.Formatting = Formatting.Indented;
                        JSONSerializer.Serialize(JsonWriterTemp, KeyJSONData);
                    }
                }
            }
            label_Status.BeginInvoke(new Action(() => { label_Status.Text = "Status: Finished writing GameParams to JSON!"; }));

            GC.Collect();

            Thread.Sleep(1000);
            label_Status.BeginInvoke(new Action(() =>
            {
                label_Status.Text = "Status: Idle";
                menuStrip_Main.Enabled = true;
                loadJSONToolStripMenuItem.Enabled = true;
            }));
        }

        private bool IsThereSomething2Sort(ref object ObjectPass)
        {
            return (ObjectPass != null && ObjectPass.GetType().GetInterface(nameof(IEnumerable)) != null && !ObjectPass.GetType().Name.Equals("String") && !ObjectPass.GetType().IsArray && !ObjectPass.GetType().Name.Equals("ArrayList")); //collection type;
        }

        private SortedDictionary<object, object> ConvertDataValue(object ObjectPass)
        {
            switch (ObjectPass.GetType().Name)
            {
                case "Hashtable":
                    {
                        return new SortedDictionary<object, object>(((Hashtable)ObjectPass).Cast<DictionaryEntry>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                    }

                case "ClassDict": //Unable to cast object of type 'Razorvine.Pickle.Objects.ClassDict' to type 'System.Collections.Generic.Dictionary`2[System.Object,System.Object]'.
                    {

                        SortedDictionary<object, object> SortedDictionaryTemp = new SortedDictionary<object, object>();
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)ObjectPass)
                        {
                            SortedDictionaryTemp.Add(kvp.Key, kvp.Value);
                        }
                        return SortedDictionaryTemp;
                    }

                default:
                    {
                        return new SortedDictionary<object, object>((Dictionary<object, object>)ObjectPass);
                    }
            }
        }

        // - - - - -

        private void LoadJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadJSONToolStripMenuItem.Enabled = false;
            //ShipHumanNames.ReadShipAPI();
            ShipHumanNames.CreateShipListWithNames();
            LoadData();
        }

        private void LoadData()
        {
            LoadProjectileData();
            LoadShipData();

            DataTable DataTableTemp = new DataTable();
            DataTableTemp.Columns.Add("Ship Name", typeof(string));
            DataTableTemp.Columns.Add("Burning Res.", typeof(string));
            DataTableTemp.Columns.Add("B. Damage Tick", typeof(string));
            DataTableTemp.Columns.Add("B. Duration", typeof(string));
            DataTableTemp.Columns.Add("B. Damage Total", typeof(string));
            DataTableTemp.Columns.Add("Flooding Res.", typeof(string));
            DataTableTemp.Columns.Add("F. Damage Tick", typeof(string));
            DataTableTemp.Columns.Add("F. Duration", typeof(string));
            DataTableTemp.Columns.Add("F. Damage Total", typeof(string));
            DataTableTemp.Columns.Add("Sigma", typeof(string));
            DataTableTemp.Columns.Add("Shell Name", typeof(string));
            DataTableTemp.Columns.Add("Ricochet Angle", typeof(string));
            DataTableTemp.Columns.Add("Always Ricochet At", typeof(string));
            DataTableTemp.Columns.Add("Air Drag Coef.", typeof(string));
            DataTableTemp.Columns.Add("Krupp", typeof(string));
            DataTableTemp.Columns.Add("Penetration", typeof(string));
            DataTableTemp.Columns.Add("Torpedo Name", typeof(string));
            DataTableTemp.Columns.Add("Damage", typeof(string));
            DataTableTemp.Columns.Add("Flood Chance", typeof(string));
            foreach (KeyValuePair<string, ShipParams> ShipData in ShipData)
            {
                DataRow DataRowTemp = DataTableTemp.NewRow();
                DataRowTemp["Ship Name"] = ShipData.Key;
                DataRowTemp["Burning Res."] = string.Join(Environment.NewLine, ShipData.Value.Burning.Select(s => s.Resistance).ToList());
                DataRowTemp["B. Damage Tick"] = string.Join("%" + Environment.NewLine, ShipData.Value.Burning.Select(s => s.PercentTick).ToList()) + "%";
                DataRowTemp["B. Duration"] = string.Join(Environment.NewLine, ShipData.Value.Burning.Select(s => s.Duration).ToList());
                DataRowTemp["B. Damage Total"] = string.Join("%" + Environment.NewLine, ShipData.Value.Burning.Select(s => s.Duration * s.PercentTick).ToList()) + "%";
                DataRowTemp["Flooding Res."] = string.Join(Environment.NewLine, ShipData.Value.Flooding.Select(s => s.Resistance).ToList());
                DataRowTemp["F. Damage Tick"] = string.Join("%" + Environment.NewLine, ShipData.Value.Flooding.Select(s => s.PercentTick).ToList()) + "%";
                DataRowTemp["F. Duration"] = string.Join(Environment.NewLine, ShipData.Value.Flooding.Select(s => s.Duration).ToList());
                DataRowTemp["F. Damage Total"] = string.Join("%" + Environment.NewLine, ShipData.Value.Flooding.Select(s => s.Duration * s.PercentTick).ToList()) + "%";
                if (ShipData.Value.Sigma.Any())
                {
                    DataRowTemp["Sigma"] = string.Join(Environment.NewLine, ShipData.Value.Sigma);
                }
                if (ShipData.Value.Shells.Any())
                {
                    DataRowTemp["Shell Name"] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.ShellName).ToList());
                    DataRowTemp["Ricochet Angle"] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.RicochetAt).ToList());
                    DataRowTemp["Always Ricochet At"] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.AlwaysRicochetAt).ToList());
                    DataRowTemp["Air Drag Coef."] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.AirDragCoef).ToList());
                    DataRowTemp["Krupp"] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.KruppCoef).ToList());
                    DataRowTemp["Penetration"] = string.Join(Environment.NewLine, ShipData.Value.Shells.Select(s => s.Penetration).ToList());
                }
                if (ShipData.Value.Torpedoes.Any())
                {
                    DataRowTemp["Torpedo Name"] = string.Join(Environment.NewLine, ShipData.Value.Torpedoes.Select(s => s.TorpName).ToList());
                    DataRowTemp["Damage"] = string.Join(Environment.NewLine, ShipData.Value.Torpedoes.Select(s => s.Damage).ToList());
                    DataRowTemp["Flood Chance"] = string.Join("%" + Environment.NewLine, ShipData.Value.Torpedoes.Select(s => (int)(s.FloodingChance * 100)).ToList()) + "%";
                }
                DataTableTemp.Rows.Add(DataRowTemp);
            }

            using (StreamWriter SteamWriterTemp = new StreamWriter("GameParams.csv", false, Encoding.UTF8))
            {
                for (int i = 0; i < DataTableTemp.Columns.Count; i++)
                {
                    SteamWriterTemp.Write(DataTableTemp.Columns[i]);
                    if (i < DataTableTemp.Columns.Count - 1)
                    {
                        SteamWriterTemp.Write(",");
                    }
                }
                SteamWriterTemp.Write(SteamWriterTemp.NewLine);

                foreach (DataRow dr in DataTableTemp.Rows)
                {
                    for (int i = 0; i < DataTableTemp.Columns.Count; i++)
                    {
                        SteamWriterTemp.Write(string.Format("\"{0}\"", dr[i].ToString()));
                        if (i < DataTableTemp.Columns.Count - 1)
                        {
                            SteamWriterTemp.Write(",");
                        }
                    }
                    SteamWriterTemp.Write(SteamWriterTemp.NewLine);
                }

            }
            MessageBox.Show("Done!", this.Text);
        }

        Dictionary<string, ShellParams> ShellData = new Dictionary<string, ShellParams>();
        Dictionary<string, TorpParams> TorpData = new Dictionary<string, TorpParams>();
        private void LoadProjectileData()
        {
            string[] FileList = Directory.GetFiles(@"JSONDumpFolder\Projectile", "*.json", SearchOption.TopDirectoryOnly);
            JObject JObjectTemp;
            foreach (string FilePath in FileList)
            {
                string FileName = FilePath.Substring(FilePath.LastIndexOf("\\") + 1);
                if (FileName.Substring(3, 1).Equals("A"))
                {
                    JObjectTemp = JObject.Parse(File.ReadAllText(FilePath));
                    if (JObjectTemp["bulletAlwaysRicochetAt"] == null) continue;
                    ShellParams ShellParamsTemp = new ShellParams
                    {
                        ShellName = JObjectTemp["name"].Value<string>().Substring(JObjectTemp["name"].Value<string>().IndexOf("_") + 1),
                        AlwaysRicochetAt = JObjectTemp["bulletAlwaysRicochetAt"].Value<double>(),
                        RicochetAt = JObjectTemp["bulletRicochetAt"].Value<double>(),
                        AirDragCoef = JObjectTemp["bulletAirDrag"].Value<double>(),
                        KruppCoef = JObjectTemp["bulletKrupp"].Value<double>(),
                        Penetration = JObjectTemp["alphaPiercingHE"].Value<double>() == 0 ? JObjectTemp["alphaPiercingCS"].Value<double>() : JObjectTemp["alphaPiercingHE"].Value<double>()
                    };
                    ShellData.Add(JObjectTemp["name"].Value<string>(), ShellParamsTemp);
                }
                if (FileName.Substring(3, 1).Equals("T"))
                {
                    JObjectTemp = JObject.Parse(File.ReadAllText(FilePath));
                    TorpParams TorpParamsTemp = new TorpParams
                    {
                        TorpName = JObjectTemp["name"].Value<string>(),
                        AlphaDamage = JObjectTemp["alphaDamage"].Value<int>(),
                        BaseDamage = JObjectTemp["damage"].Value<int>(),
                        FloodingChance = JObjectTemp["uwCritical"].Value<double>(),
                        Range = JObjectTemp["maxDist"].Value<int>() * 30,
                        Speed = JObjectTemp["speed"].Value<double>(),
                        Visibility = JObjectTemp["visibilityFactor"].Value<double>()
                    };
                    TorpParamsTemp.Damage = TorpParamsTemp.AlphaDamage / 3 + TorpParamsTemp.BaseDamage;
                    TorpData.Add(JObjectTemp["name"].Value<string>(), TorpParamsTemp);
                }
            }
        }

        Dictionary<string, ShipParams> ShipData = new Dictionary<string, ShipParams>();
        private void LoadShipData()
        {
            string[] FileList = Directory.GetFiles(@"JSONDumpFolder\Ship", "*.json", SearchOption.TopDirectoryOnly);

            foreach (string FilePath in FileList)
            {
                JObject JObjectTemp = JObject.Parse(File.ReadAllText(FilePath));

                string ShipName = JObjectTemp["name"].Value<string>();
                ShipName = ShipName.Substring(0, ShipName.IndexOf("_"));

                //Submarines not in human ship list names yet so add them
                if (ShipName[3].Equals("S")) 
                {
                    if (!ShipHumanNames.ShipListNames.ContainsKey(ShipName))
                    {
                        ShipHumanNames.ShipListNames.Add(ShipName, JObjectTemp["name"].Value<string>().Replace(ShipName + "_", ""));
                    }
                }

                //if (!ShipHumanNames.ShipListNames.ContainsKey(ShipName)) continue; //Skip non-player ships

                ShipParams ShipParamsTemp = new ShipParams();
                List<JToken> ProjectileList = new List<JToken>();
                foreach (JToken JTokenTemp in JObjectTemp.Children())
                {
                    if (JTokenTemp.First.Type != JTokenType.Object) continue;

                    if (JTokenTemp.First["ammoPool"] != null) //Shell
                    {
                        ProjectileList.Add(JTokenTemp);
                        continue;
                    }
                    if (JTokenTemp.First["armourCit"] != null) //Hull
                    {
                        BurnNFloodParams BurnNFloodParamsTemp0 = new BurnNFloodParams
                        {
                            Resistance = JTokenTemp.First["burnNodes"][0][0].Value<double>(),
                            PercentTick = JTokenTemp.First["burnNodes"][0][1].Value<double>(),
                            Duration = JTokenTemp.First["burnNodes"][0][2].Value<int>()
                        };
                        ShipParamsTemp.Burning.Add(BurnNFloodParamsTemp0);
                        BurnNFloodParams BurnNFloodParamsTemp1 = new BurnNFloodParams
                        {
                            Resistance = JTokenTemp.First["floodNodes"][0][0].Value<double>(),
                            PercentTick = JTokenTemp.First["floodNodes"][0][1].Value<double>(),
                            Duration = JTokenTemp.First["floodNodes"][0][2].Value<int>()
                        };
                        ShipParamsTemp.Flooding.Add(BurnNFloodParamsTemp1);
                        continue;
                    }
                    if (JTokenTemp.First["useOneShot"] != null) //Torp
                    {
                        ProjectileList.Add(JTokenTemp);
                        continue;
                    }
                }

                if (ProjectileList.Any())
                {
                    List<string> ShipShells = new List<string>();
                    List<string> ShipTorps = new List<string>();
                    List<double> SigmaValues = new List<double>();
                    foreach (JToken JTokenTemp1 in ProjectileList)
                    {
                        foreach (JProperty JTokenTemp2 in JTokenTemp1.First.Children())
                        {
                            if (!JTokenTemp2.Name.StartsWith("HP_")) continue;

                            foreach (JToken JTokenTemp3 in JTokenTemp2.First["ammoList"])
                            {
                                string AmmoType = JTokenTemp3.ToString().Substring(3, 1);
                                switch (AmmoType)
                                {
                                    case "A":
                                        {
                                            //if (JTokenTemp3.Value<string>().Contains("_HE")) continue;
                                            ShipShells.Add(JTokenTemp3.Value<string>());
                                            break;
                                        }

                                    case "T":
                                        {
                                            ShipTorps.Add(JTokenTemp3.Value<string>());
                                            break;
                                        }
                                }
                            }
                        }
                        if (JTokenTemp1.First["sigmaCount"] != null)
                        {
                            SigmaValues.Add(JTokenTemp1.First["sigmaCount"].Value<double>());
                        }
                    }

                    ShipShells = ShipShells.Distinct().ToList();
                    foreach (string ShellName in ShipShells)
                    {
                        //if (ShellData[ShellName].Penetration == 0) continue;
                        ShipParamsTemp.Shells.Add(ShellData[ShellName]);
                    }
                    ShipTorps = ShipTorps.Distinct().ToList();
                    foreach (string TorpName in ShipTorps)
                    {
                        ShipParamsTemp.Torpedoes.Add(TorpData[TorpName]);
                    }
                    SigmaValues = SigmaValues.Distinct().ToList();
                    ShipParamsTemp.Sigma = SigmaValues;
                }
                ShipData.Add(ShipHumanNames.ShipListNames[ShipName], ShipParamsTemp);
            }
        }

        private void UpdateShipListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateShipListToolStripMenuItem.Enabled = false;
            ShipHumanNames.ReadShipAPI();
        }
    }

    public class ShipParams
    {
        public List<double> Sigma = new List<double>();
        public List<ShellParams> Shells = new List<ShellParams>();
        public List<TorpParams> Torpedoes = new List<TorpParams>();
        public List<BurnNFloodParams> Burning = new List<BurnNFloodParams>();
        public List<BurnNFloodParams> Flooding = new List<BurnNFloodParams>();
    }

    public class ShellParams
    {
        public string ShellName;
        public double AlwaysRicochetAt;
        public double RicochetAt;
        public double AirDragCoef;
        public double KruppCoef;
        public double Penetration;
    }

    public class BurnNFloodParams
    {
        public double Resistance;
        public double PercentTick;
        public int Duration;
    }

    public class TorpParams
    {
        public string TorpName;
        public int AlphaDamage;
        public int BaseDamage;
        public int Damage; //Alpha/3 + base
        public double FloodingChance;
        public double Speed;
        public int Range; //x * 30
        public double Visibility;
    }

}
