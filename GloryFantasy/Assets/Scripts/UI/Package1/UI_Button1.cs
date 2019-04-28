/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
	public partial class UI_Button1 : GButton
	{
		public Controller m_button;
		public GImage m_n0;
		public GImage m_n1;
		public GImage m_n2;

		public const string URL = "ui://wty7y520n7i24";

		public static UI_Button1 CreateInstance()
		{
			return (UI_Button1)UIPackage.CreateObject("Package1","Button1");
		}

		public UI_Button1()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_button = this.GetControllerAt(0);
			m_n0 = (GImage)this.GetChildAt(0);
			m_n1 = (GImage)this.GetChildAt(1);
			m_n2 = (GImage)this.GetChildAt(2);
		}
	}
}