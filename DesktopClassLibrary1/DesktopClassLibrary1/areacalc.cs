using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace DesktopClassLibrary1
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("d78fc03b-3e29-4537-9416-752e34403120")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("DesktopClassLibrary1.areacalc")]
    public sealed class areacalc : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }

        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }
        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;

        public areacalc()
        {
            base.m_category = "DIY��չ��"; // ������������𣨱��ػ��ı���
            base.m_caption = "DIY��չ�������";  // ������ʾ�����ƣ����ػ��ı���
            base.m_message = "����˹��߼���ͼ���������½��ֶβ����м���";  // ������ʾ��Ϣ�����ػ��ı���
            base.m_toolTip = "����������ߣ�����˹��߼���ͼ���������½��ֶβ����м���";  // �����ͣʱ����ʾ��Ϣ�����ػ��ı���
            base.m_name = "DesktopClassLibrary1_areacalc";   // Ψһ��ʶ�������ɱ��ػ������� "MyCategory_MyCommand"��

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
            {
                MessageBox.Show("Hook ����Ϊ��");
                return;
            }

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    MessageBox.Show("ActiveView Ϊ��");
                    m_hookHelper = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("��ʼ�� m_hookHelper ����: " + ex.Message);
                m_hookHelper = null;
            }

            base.m_enabled = (m_hookHelper != null);
            MessageBox.Show("��������״̬: " + base.m_enabled);
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                // ��� m_hookHelper �Ƿ��ʼ��
                if (m_hookHelper == null)
                {
                    MessageBox.Show("m_hookHelper δ��ʼ��");
                    return;
                }

                // ��ȡ���ͼ
                IMap pMap = m_hookHelper.ActiveView as IMap;
                if (pMap == null)
                {
                    MessageBox.Show("δ�ҵ����ͼ");
                    return;
                }

                // ����ͼ���Ƿ���ͼ��
                if (pMap.LayerCount == 0)
                {
                    MessageBox.Show("��ͼ��û��ͼ��");
                    return;
                }

                // ��ȡ��һ��ͼ��
                ILayer pLayer = pMap.get_Layer(0);
                if (!(pLayer is IFeatureLayer))
                {
                    MessageBox.Show("��ǰͼ�㲻��ʸ��ͼ��");
                    return;
                }

                // ת��ΪҪ��ͼ��
                IFeatureLayer pFeatureLayer = (IFeatureLayer)pLayer;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

                // �������ֶ�
                IField pField = new Field();
                IFieldEdit pFieldEdit = (IFieldEdit)pField;
                int IFieldID = pFeatureClass.FindField("area");

                if (IFieldID == -1)
                {
                    pFieldEdit.Name_2 = "area";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                    pFieldEdit.Precision_2 = 14;
                    pFieldEdit.Scale_2 = 4;
                    pFieldEdit.Length_2 = 10;
                    pFieldEdit.AliasName_2 = "���";

                    pFeatureClass.AddField(pField);

                    // ���»�ȡ�ֶ�����
                    IFieldID = pFeatureClass.FindField("area");
                    if (IFieldID == -1)
                    {
                        MessageBox.Show("����ֶ�ʧ��");
                        return;
                    }
                }

                // �������
                IFeatureCursor pFeatureCursor = pFeatureClass.Update(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    IPolygon pPolygon = pFeature.Shape as IPolygon;
                    if (pPolygon == null)
                    {
                        MessageBox.Show("Ҫ����״���Ƕ����");
                        return;
                    }

                    IArea pArea = pPolygon as IArea;
                    if (pArea == null)
                    {
                        MessageBox.Show("�޷��������");
                        return;
                    }

                    pFeature.set_Value(IFieldID, pArea.Area);
                    pFeatureCursor.UpdateFeature(pFeature);
                    pFeature = pFeatureCursor.NextFeature();
                }

                MessageBox.Show("����������");
            }
            catch (Exception ex)
            {
                MessageBox.Show("����ִ�г���: " + ex.Message);
            }
        }

        #endregion
    }
}