/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
	public partial class UI_Component1 : GComponent
	{
		public GImage m_n0;
		public UI_Button1 m_n1;
		public Transition m_t0;
		public Transition m_t1;

		public const string URL = "ui://wty7y520n7i20";

		public static UI_Component1 CreateInstance()
		{
			return (UI_Component1)UIPackage.CreateObject("Package1","Component1");
		}

		public UI_Component1()
		{
		}

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			m_n0 = (GImage)this.GetChildAt(0);
			m_n1 = (UI_Button1)this.GetChildAt(1);
			m_t0 = this.GetTransitionAt(0);
			m_t1 = this.GetTransitionAt(1);
		}
	}
}