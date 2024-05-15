using System;
using NitroxClient.Debuggers.Drawer.Unity;
using UnityEngine;

namespace NitroxClient.Debuggers.Drawer.UnityUI;

public class RectTransformDrawer : IDrawer<RectTransform>
{
    private readonly VectorDrawer vectorDrawer;
    private const float LABEL_WIDTH = 120;
    private const float VECTOR_MAX_WIDTH = 405;

    public RectTransformDrawer(VectorDrawer vectorDrawer)
    {
        this.vectorDrawer = vectorDrawer;
    }

    public void Draw(RectTransform rectTransform)
    {
        using (new GUILayout.VerticalScope())
        {
            //TODO: Implement position display like the Unity editor
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Anchored Position", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.anchoredPosition = vectorDrawer.Draw(rectTransform.anchoredPosition, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH));
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Local Position", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.localPosition = vectorDrawer.Draw(rectTransform.localPosition, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH));
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Local  Rotation", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.localRotation = Quaternion.Euler(vectorDrawer.Draw(rectTransform.localRotation.eulerAngles, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH)));
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Local  Scale", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.localScale = vectorDrawer.Draw(rectTransform.localScale, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH));
            }

            GUILayout.Space(20);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Size", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.sizeDelta = vectorDrawer.Draw(rectTransform.sizeDelta, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH));
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Anchor", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                AnchorMode anchorMode = VectorToAnchorMode(rectTransform.anchorMin, rectTransform.anchorMax);

                if (anchorMode == AnchorMode.NONE)
                {
                    vectorDrawer.Draw(rectTransform.anchorMin, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH * 0.5f));
                    vectorDrawer.Draw(rectTransform.anchorMax, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH * 0.5f));
                }
                else
                {
                    anchorMode = NitroxGUILayout.EnumPopup(anchorMode, VECTOR_MAX_WIDTH);

                    // Vector2[] anchorVectors = AnchorModeToVector(anchorMode);
                    // rectTransform.anchorMin = anchorVectors[0];
                    // rectTransform.anchorMax = anchorVectors[1];
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Pivot", NitroxGUILayout.DrawerLabel, GUILayout.Width(LABEL_WIDTH));
                NitroxGUILayout.Separator();
                rectTransform.pivot = vectorDrawer.Draw(rectTransform.pivot, new VectorDrawer.DrawOptions(VECTOR_MAX_WIDTH));
            }
        }
    }

    private enum AnchorMode
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        TOP_STRETCH,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        MIDDLE_STRETCH,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
        BOTTOM_STRETCH,
        STRETCH_LEFT,
        STRETCH_CENTER,
        STRETCH_RIGHT,
        STRETCH_STRETCH,
        NONE
    }

    private const float FLOAT_TOLERANCE = 0.0001f;

#pragma warning disable IDE0011 // ReSharper disable EnforceIfStatementBraces
    private static AnchorMode VectorToAnchorMode(Vector2 min, Vector2 max)
    {
        bool minXNull = min.x == 0f;
        bool minXHalf = Math.Abs(min.x - 0.5f) < FLOAT_TOLERANCE;
        bool minXFull = Math.Abs(min.x - 1f) < FLOAT_TOLERANCE;

        bool minYNull = min.y == 0f;
        bool minYHalf = Math.Abs(min.y - 0.5f) < FLOAT_TOLERANCE;
        bool minYFull = Math.Abs(min.y - 1f) < FLOAT_TOLERANCE;

        bool maxXNull = max.x == 0f;
        bool maxXHalf = Math.Abs(max.x - 0.5f) < FLOAT_TOLERANCE;
        bool maxXFull = Math.Abs(max.x - 1f) < FLOAT_TOLERANCE;

        bool maxYNull = max.y == 0f;
        bool maxYHalf = Math.Abs(max.y - 0.5f) < FLOAT_TOLERANCE;
        bool maxYFull = Math.Abs(max.y - 1f) < FLOAT_TOLERANCE;

        if (minYFull && maxYFull)
        {
            if (minXNull && maxXNull)
                return AnchorMode.TOP_LEFT;
            if (minXHalf && maxXHalf)

                return AnchorMode.TOP_CENTER;
            if (minXFull && maxXFull)
                return AnchorMode.TOP_RIGHT;
            if (minXNull && maxXFull)
                return AnchorMode.TOP_STRETCH;
        }

        if (minYHalf && maxYHalf)
        {
            if (minXNull && maxXNull)
                return AnchorMode.MIDDLE_LEFT;
            if (minXHalf && maxXHalf)
                return AnchorMode.MIDDLE_CENTER;
            if (minXFull && maxXFull)
                return AnchorMode.MIDDLE_RIGHT;
            if (minXNull && maxXFull)
                return AnchorMode.MIDDLE_STRETCH;
        }

        if (minYNull && maxYNull)
        {
            if (minXNull && maxXNull)
                return AnchorMode.BOTTOM_LEFT;
            if (minXHalf && maxXHalf)
                return AnchorMode.BOTTOM_CENTER;
            if (minXFull && maxXFull)
                return AnchorMode.BOTTOM_RIGHT;
            if (minXNull && maxXFull)
                return AnchorMode.BOTTOM_STRETCH;
        }

        if (minYNull && maxYFull)
        {
            if (minXNull && maxXNull)
                return AnchorMode.STRETCH_LEFT;
            if (minXHalf && maxXHalf)
                return AnchorMode.STRETCH_CENTER;
            if (minXFull && maxXFull)
                return AnchorMode.STRETCH_RIGHT;
            if (minXNull && maxXFull)
                return AnchorMode.STRETCH_STRETCH;
        }

        return AnchorMode.NONE;
    }
#pragma warning restore IDE0011 // ReSharper restore EnforceIfStatementBraces

    private static Vector2[] AnchorModeToVector(AnchorMode anchorMode) =>
        anchorMode switch
        {
            AnchorMode.TOP_LEFT => [new Vector2(0, 1), new Vector2(0, 1)],
            AnchorMode.TOP_CENTER => [new Vector2(0.5f, 1), new Vector2(0.5f, 1)],
            AnchorMode.TOP_RIGHT => [new Vector2(1, 1), new Vector2(1, 1)],
            AnchorMode.TOP_STRETCH => [new Vector2(0, 1), new Vector2(1, 1)],
            AnchorMode.MIDDLE_LEFT => [new Vector2(0, 0.5f), new Vector2(0, 0.5f)],
            AnchorMode.MIDDLE_CENTER => [new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f)],
            AnchorMode.MIDDLE_RIGHT => [new Vector2(1, 0.5f), new Vector2(1, 0.5f)],
            AnchorMode.MIDDLE_STRETCH => [new Vector2(0, 0.5f), new Vector2(1, 0.5f)],
            AnchorMode.BOTTOM_LEFT => [new Vector2(0, 0), new Vector2(0, 0)],
            AnchorMode.BOTTOM_CENTER => [new Vector2(0.5f, 0), new Vector2(0.5f, 0)],
            AnchorMode.BOTTOM_RIGHT => [new Vector2(1, 0), new Vector2(1, 0)],
            AnchorMode.BOTTOM_STRETCH => [new Vector2(0, 0), new Vector2(1, 0)],
            AnchorMode.STRETCH_LEFT => [new Vector2(0, 0), new Vector2(0, 1)],
            AnchorMode.STRETCH_CENTER => [new Vector2(0.5f, 0), new Vector2(0.5f, 1)],
            AnchorMode.STRETCH_RIGHT => [new Vector2(1, 0), new Vector2(1, 1)],
            AnchorMode.STRETCH_STRETCH => [new Vector2(0, 0), new Vector2(1, 1)],
            AnchorMode.NONE => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
}
