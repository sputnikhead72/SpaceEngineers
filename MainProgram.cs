using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;



namespace SpaceEngineers
{
	class MainProgram
	{
#region Predefined Variables

		IMyGridTerminalSystem GridTerminalSystem = null;
		string powerpanelname = "Power Panel";


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
		public void WritePowerInfo (string textpanelname)
		{
			new PowerManagement ().WritePowerInfo (textpanelname);
		}

#endregion

		#region Code Editor

		public void Main (string argument)
		{
			try
			{
				WritePowerInfo (powerpanelname);
			}

			catch (Exception ex)
			{
				WriteTextToPanel (powerpanelname, "");

				foreach (object key in ex.Data.Keys)
				{
					WriteTextToPanel (powerpanelname, "{0\" = \"{1}\"\r\n", true, key, ex.Data [key]);
				}
			}
		}

#endregion
	}
}
