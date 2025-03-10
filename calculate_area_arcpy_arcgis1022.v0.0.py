#coding:--utf-8--

import arcpy as ap
import math
import numpy as np
import time
import matplotlib as plt
from CalculateAreas import areaFieldName

infile = ap.GetParameterAsText(0)
area_Field_Name =ap.GetParameterAsText(1)

desc = ap.Describe(infile)
if desc.dataType == "ShapeFile":
    print u"当前数据为shapefile"
if desc.datasetType == "FeatureClass":
    print u"0"
if desc.FeatureType == "Simple":
    print u"0"
if desc.ShapeType == "Polygon":
    print u"当前图层是多边形图层"
elif desc.ShapeType == "Polyline":
    print u"当前图层是线图层"
elif desc.ShapeType == "Point":
    print u"当前图层是点图层"
elif desc.ShapeType == "MultiPoint":
    print u"0"
else:
    print u"0"


if area_Field_Name.encode("utf-8") not in [s.name.encode("utf-8") for s in desc.Fields]:
    ap.AddField_management(infile,area_Field_Name,"DOUBLE")
else:
    print u"0"


#获取游标
cursor=ap.da.UpdateCursor(infile,["SHAPE@",area_Field_Name])
for row in cursor:
    row[1] = row[0].area
    cursor.updateRow(row)

print u"finished!"
