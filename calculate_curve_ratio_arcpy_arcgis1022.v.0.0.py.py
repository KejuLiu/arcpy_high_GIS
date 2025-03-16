import arcpy as ap
'''
mxd=ap.mapping.MapDocument("CURRENT")
lyr=ap.mapping.ListLayers(mxd)
'''
lyr =ap.GetParameterAsText(0)
desc=ap.Describe(lyr)

if not "wqd" in desc.Fields:
    ap.AddField_management(lyr,"wqd","DOUBLE")
'''
if desc.ShapeType=="Polyline":
    raise (u"当前图层不是线图层")
'''
cur=ap.UpdateCursor(lyr)
for row in cur:
    geom=row.shape
    firstPoint=ap.PointGeometry(geom.firstPoint)
    lastPoint=ap.PointGeometry(geom.lastPoint)
    dis=firstPoint.distanceTo(lastPoint)
    if dis == 0:
        row.setValue("wqd",10000)
    else :
        row.setValue("wqd",geom.length/dis)
        cur.updateRow(row)
