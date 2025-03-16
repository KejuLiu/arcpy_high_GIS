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
            base.m_category = "DIY拓展包"; // 工具所属的类别（本地化文本）
            base.m_caption = "DIY拓展计算面积";  // 工具显示的名称（本地化文本）
            base.m_message = "点击此工具计算图层的面积，新建字段并进行计算";  // 工具提示信息（本地化文本）
            base.m_toolTip = "计算面积工具，点击此工具计算图层的面积，新建字段并进行计算";  // 鼠标悬停时的提示信息（本地化文本）
            base.m_name = "DesktopClassLibrary1_areacalc";   // 唯一标识符，不可本地化（例如 "MyCategory_MyCommand"）

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
                MessageBox.Show("Hook 对象为空");
                return;
            }

            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    MessageBox.Show("ActiveView 为空");
                    m_hookHelper = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化 m_hookHelper 出错: " + ex.Message);
                m_hookHelper = null;
            }

            base.m_enabled = (m_hookHelper != null);
            MessageBox.Show("工具启用状态: " + base.m_enabled);
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                // 检查 m_hookHelper 是否初始化
                if (m_hookHelper == null)
                {
                    MessageBox.Show("m_hookHelper 未初始化");
                    return;
                }

                // 获取活动地图
                IMap pMap = m_hookHelper.ActiveView as IMap;
                if (pMap == null)
                {
                    MessageBox.Show("未找到活动地图");
                    return;
                }

                // 检查地图中是否有图层
                if (pMap.LayerCount == 0)
                {
                    MessageBox.Show("地图中没有图层");
                    return;
                }

                // 获取第一个图层
                ILayer pLayer = pMap.get_Layer(0);
                if (!(pLayer is IFeatureLayer))
                {
                    MessageBox.Show("当前图层不是矢量图层");
                    return;
                }

                // 转换为要素图层
                IFeatureLayer pFeatureLayer = (IFeatureLayer)pLayer;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

                // 添加面积字段
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
                    pFieldEdit.AliasName_2 = "面积";

                    pFeatureClass.AddField(pField);

                    // 重新获取字段索引
                    IFieldID = pFeatureClass.FindField("area");
                    if (IFieldID == -1)
                    {
                        MessageBox.Show("添加字段失败");
                        return;
                    }
                }

                // 计算面积
                IFeatureCursor pFeatureCursor = pFeatureClass.Update(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    IPolygon pPolygon = pFeature.Shape as IPolygon;
                    if (pPolygon == null)
                    {
                        MessageBox.Show("要素形状不是多边形");
                        return;
                    }

                    IArea pArea = pPolygon as IArea;
                    if (pArea == null)
                    {
                        MessageBox.Show("无法计算面积");
                        return;
                    }

                    pFeature.set_Value(IFieldID, pArea.Area);
                    pFeatureCursor.UpdateFeature(pFeature);
                    pFeature = pFeatureCursor.NextFeature();
                }

                MessageBox.Show("面积计算完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("工具执行出错: " + ex.Message);
            }
        }

        #endregion
    }
}