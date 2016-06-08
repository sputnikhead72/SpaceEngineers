using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace SpaceEngineers
{
	public class PowerManagement
	{
#region Predefined Variables

		IMyGridTerminalSystem GridTerminalSystem = null;

		public void WriteTextToPanel (string panelname, string text, bool append = false)
		{
			new Display ().WriteTextToPanel (panelname, text, append);
		}
		public void WriteTextToPanel (string panelname, string textformat, bool append, params object [] args)
		{
			new Display ().WriteTextToPanel (panelname, textformat, append, args);
		}
		public void WriteTextToPanels (string panelname, string text, bool append = false)
		{
			new Display ().WriteTextToPanels (panelname, text, append);
		}
		public void WriteTextToPanels (string panelname, string textformat, bool append, params object [] args)
		{
			new Display ().WriteTextToPanels (panelname, textformat, append, args);
		}

#endregion

#region Code Editor

		public struct PowerInfo
		{
			public double MaxWatts;
			public double Watts;



			private const string Prefixes = "kMGTEYZ";



			public PowerInfo (double max, double watts)
			{
				MaxWatts = max;
				Watts = output;
			}



			public PowerInfo (IMySolarPanel panel) : this (0, 0)
			{
				ParseFromDetailedInfo (panel);
			}



			public PowerInfo (IMyReactor reactor) : this (0, 0)
			{
				ParseFromDetailedInfo (reactor);
			}



			private double GetWattValue (string text)
			{
				string [] parts = text.Split (new string [] {" "}, StringSplitOptions.None);
				double watts;
				double exponent;


				if (parts.Length < 2)
				{
					return (0.0);
				}

				watts = Convert.ToDouble (parts [0]);
				exponent = (Prefixes.IndexOf (parts [1].Substring (0, 1)) + 1) * 3;
				watts *= Math.Pow (10, exponent);


				return (watts);
			}



			private void ParseFromDetailedInfo (IMySolarPanel panel)
			{
				string [] lines = panel.DetailedInfo.Split (new char [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
				string [] [] dictionary = new string [lines.Length] [];
				string maxoutput = "";
				string currentoutput = "";

				for (int i = 0; i < lines.Length; i ++)
				{
					dictionary [i] = lines [i].Split (new string [] { ": " }, StringSplitOptions.None);

					switch (dictionary [i] [0])
					{
						case "Max Output":
							maxoutput = dictionary [i] [1];
							break;

						case "Current Output":
							currentoutput = dictionary [i] [1];
							break;
					}
				}

				MaxWatts = GetWattValue (maxoutput);
				Watts = GetWattValue (currentoutput);
			}



			private void ParseFromDetailedInfo (IMyReactor reactor)
			{
				string [] lines = reactor.DetailedInfo.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				string [] [] dictionary = new string [lines.Length] [];
				string maxoutput = "";
				string currentoutput = "";

				for (int i = 0; i < lines.Length; i++)
				{
					dictionary [i] = lines [i].Split (new string [] { ": " }, StringSplitOptions.None);

					switch (dictionary [i] [0])
					{
						case "Max Output":
							maxoutput = dictionary [i] [1];
							break;

						case "Current Output":
							currentoutput = dictionary [i] [1];
							break;
					}
				}

				MaxWatts = GetWattValue (maxoutput);
				Watts = GetWattValue (currentoutput);
			}



			private void ParseOutputFromDetailedInfo (IMyBatteryBlock battery)
			{
				string [] lines = battery.DetailedInfo.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				string [] [] dictionary = new string [lines.Length] [];
				string maxoutput = "";
				string currentoutput = "";

				for (int i = 0; i < lines.Length; i++)
				{
					dictionary [i] = lines [i].Split (new string [] { ": " }, StringSplitOptions.None);

					switch (dictionary [i] [0])
					{
						case "Max Output":
							maxoutput = dictionary [i] [1];
							break;

						case "Current Output":
							currentoutput = dictionary [i] [1];
							break;
					}
				}

				MaxWatts = GetWattValue (maxoutput);
				Watts = GetWattValue (currentoutput);
			}



			public static PowerInfo operator + (PowerInfo arg1, PowerInfo arg2)
			{
				PowerInfo result = new PowerInfo (arg1.MaxWatts, arg1.Watts);

				result.MaxWatts += arg2.MaxWatts;
				result.Watts += arg2.Watts;


				return (result);
			}



			private string WattsToString (double watts, bool withprefixes = true)
			{
				string text = String.Empty;
				int prefixindex = 0;
				string prefix = String.Empty;


				while (watts > 1000)
				{
					watts /= 1000;
					prefixindex ++;
				}

				if (prefixindex > 0)
				{
					prefix = Prefixes.Substring (prefixindex - 1, 1);
				}


				text = String.Format ("{0:0.0} {1}W", watts, prefix);


				return (text);
			}



			public string ToString (bool withprefixes)
			{
				string text = String.Format ("{0}/{1} ({2:0.0%})", WattsToString (Watts, withprefixes), WattsToString (MaxWatts, withprefixes), ((MaxWatts <= 0.0) ? 0.0 : Watts / MaxWatts));

				return (text);
			}



			public override string ToString ()
			{
				return (ToString (true));
			}
		}



		public void WritePowerInfo (string textpanelname)
		{
			PowerInfo powertotal = new PowerInfo (0, 0);
			PowerInfo power;
			List <IMySolarPanel> panels = GetSolarPanelList ();
			List <IMyReactor> reactors = GetReactorList ();

			foreach (IMySolarPanel panel in panels)
			{
				power = new PowerInfo (panel);
				powertotal += power;
			}


			WriteTextToPanel (textpanelname, "", false);
			WriteTextToPanel (textpanelname, "Solar Power Output: {0} for {1} panels", true, powertotal, panels.Count);


			powertotal = new PowerInfo (0, 0);

			foreach (IMyReactor reactor in reactors)
			{
				power = new PowerInfo (reactor);
				powertotal += power;
			}


			WriteTextToPanel (textpanelname, "", false);
			WriteTextToPanel (textpanelname, "Reactor Power Output: {0} for {1} reactors", true, powertotal, reactors.Count);


			powertotal = new PowerInfo (0, 0);
		}



		public List <T> GetBlockTypeList <T> () where T : IMyTerminalBlock
		{
			List <IMyTerminalBlock> blocks = new List <IMyTerminalBlock> ();
			List <T> typeblocks = new List <T> ();

			GridTerminalSystem.GetBlocksOfType <IMySolarPanel> (blocks);


			foreach (IMyTerminalBlock block in blocks)
			{
				typeblocks.Add ((T) (block));
			}


			return (typeblocks);
		}



		public List <IMySolarPanel> GetSolarPanelList ()
		{
			List <IMySolarPanel> panels = new List <IMySolarPanel> ();
			List <IMyTerminalBlock> blocks = new List <IMyTerminalBlock> ();


			GridTerminalSystem.GetBlocksOfType <IMySolarPanel> (blocks);


			foreach (IMyTerminalBlock block in blocks)
			{
				panels.Add ((IMySolarPanel) (block));
			}


			return (panels);
		}



		public List <IMyReactor> GetReactorList ()
		{
			List <IMyReactor> reactors = new List <IMyReactor> ();
			List <IMyTerminalBlock> blocks = new List <IMyTerminalBlock> ();


			GridTerminalSystem.GetBlocksOfType <IMySolarPanel> (blocks);


			foreach (IMyTerminalBlock block in blocks)
			{
				reactors.Add ((IMyReactor) (block));
			}


			return (reactors);
		}



		public List <IMyBatteryBlock> GetBatteryList ()
		{
			List <IMyBatteryBlock> batteries = new List <IMyBatteryBlock> ();
			List <IMyTerminalBlock> blocks = new List <IMyTerminalBlock> ();


			GridTerminalSystem.GetBlocksOfType <IMySolarPanel> (blocks);


			foreach (IMyTerminalBlock block in blocks)
			{
				batteries.Add ((IMyBatteryBlock) (block));
			}


			return (batteries);
		}

#endregion
	}
}
