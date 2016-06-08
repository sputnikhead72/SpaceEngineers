using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers
{
	public class Display
	{
#region Predefined Variables

		IMyGridTerminalSystem GridTerminalSystem = null;

#endregion

#region Code Editor

		public void WriteTextToPanel (string panelname, string text, bool append = false)
		{
			IMyTextPanel textpanel;

			try
			{
				textpanel = (IMyTextPanel) (GridTerminalSystem.GetBlockWithName (panelname));
			}

			catch (InvalidCastException ex)
			{
				throw (new InvalidCastException (String.Format ("The block with name \"{0}\" is not of type IMyTextPanel."), ex));
			}

			if (textpanel != null)
			{
				textpanel.WritePublicText (text, append);
				textpanel.ShowTextureOnScreen ();
				textpanel.ShowPublicTextOnScreen ();
			}

			else
			{
				throw (new ArgumentException (String.Format ("The name \"{0}\" does not match any blocks in the current grid."), "panelname"));
			}
		}



		public void WriteTextToPanel (string panelname, string textformat, bool append, params object [] args)
		{
			WriteTextToPanel (panelname, String.Format (textformat, args), append);
		}



		public void WriteTextToPanels (string panelname, string text, bool append = false)
		{
			List <IMyTerminalBlock> textpanels = new List <IMyTerminalBlock> ();
			IMyTextPanel p;
			GridTerminalSystem.GetBlocksOfType <IMyTextPanel> (textpanels);

			foreach (IMyTerminalBlock panel in textpanels)
			{
				p = (IMyTextPanel) (panel);

				if (p.CustomName == panelname)
				{
					p.WritePublicText (text, append);
					p.ShowTextureOnScreen ();
					p.ShowPublicTextOnScreen ();
				}
			}
		}



		public void WriteTextToPanels (string panelname, string textformat, bool append, params object [] args)
		{
			WriteTextToPanels (panelname, String.Format (textformat, args), append);
		}

#endregion
	}
}
