using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using System.Threading;
using ColossalFramework.Math;

namespace SkylinesBulldoze
{
    public class BetterBulldozeTool : ToolBase
    {
        private object m_dataLock;

        private bool m_active;
        private Vector3 m_startPosition;
        private Vector3 m_startDirection;
        private Vector3 m_mousePosition;
        private Vector3 m_mouseDirection;
        private bool m_validPosition;
        private bool m_mouseRayValid;
        private Ray m_mouseRay;
        private float m_mouseRayLength;
        private Vector3 m_cameraDirection;

        protected override void Awake()
        {
            this.m_dataLock = new object();
            m_active = true;
            base.Awake();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            while (!Monitor.TryEnter(this.m_dataLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
            {
            }
            Vector3 startPosition;
            Vector3 mousePosition;
            Vector3 startDirection;
            Vector3 mouseDirection;
            bool active;

            try
            {
                active = this.m_active;

                startPosition = this.m_startPosition;
                mousePosition = this.m_mousePosition;
                startDirection = this.m_startDirection;
                mouseDirection = this.m_mouseDirection;
            }
            finally
            {
                Monitor.Exit(this.m_dataLock);
            }

            var color = Color.red;

            if (!active)
            {
                base.RenderOverlay(cameraInfo);
                return;
            }

            Vector3 a = (!active) ? mousePosition : startPosition;
            Vector3 vector = mousePosition;
            Vector3 a2 = (!active) ? mouseDirection : startDirection;
            Vector3 a3 = new Vector3(a2.z, 0f, -a2.x);
            float num = Mathf.Round(((vector.x - a.x) * a2.x + (vector.z - a.z) * a2.z) * 0.125f) * 8f;
            float num2 = Mathf.Round(((vector.x - a.x) * a3.x + (vector.z - a.z) * a3.z) * 0.125f) * 8f;
            float num3 = (num < 0f) ? -4f : 4f;
            float num4 = (num2 < 0f) ? -4f : 4f;
            Quad3 quad = default(Quad3);
            quad.a = a - a2 * num3 - a3 * num4;
            quad.b = a - a2 * num3 + a3 * (num2 + num4);
            quad.c = a + a2 * (num + num3) + a3 * (num2 + num4);
            quad.d = a + a2 * (num + num3) - a3 * num4;
            if (num3 != num4)
            {
                Vector3 b = quad.b;
                quad.b = quad.d;
                quad.d = b;
            }
            ToolManager toolManager = ToolManager.instance;
            toolManager.m_drawCallData.m_overlayCalls++;
            RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, color, quad, -1f, 1025f, false, true);                        
            base.RenderOverlay(cameraInfo);
            return;
        }

        protected override void OnToolLateUpdate()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 cameraDirection = Vector3.Cross(Camera.main.transform.right, Vector3.up);
            cameraDirection.Normalize();
            while (!Monitor.TryEnter(this.m_dataLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
            {
            }
            try
            {
                this.m_mouseRay = Camera.main.ScreenPointToRay(mousePosition);
                this.m_mouseRayLength = Camera.main.farClipPlane;
                this.m_cameraDirection = cameraDirection;
                this.m_mouseRayValid = (!this.m_toolController.IsInsideUI && Cursor.visible);
            }
            finally
            {
                Monitor.Exit(this.m_dataLock);
            }
        }

        public override void SimulationStep()
        {
            while (!Monitor.TryEnter(this.m_dataLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
            {
            }
            Ray mouseRay;
            Vector3 cameraDirection;
            bool mouseRayValid;
            try
            {
                mouseRay = this.m_mouseRay;
                cameraDirection = this.m_cameraDirection;
                mouseRayValid = this.m_mouseRayValid;
            }
            finally
            {
                Monitor.Exit(this.m_dataLock);
            }

            ToolBase.RaycastInput input = new ToolBase.RaycastInput(mouseRay, m_mouseRayLength);
            ToolBase.RaycastOutput raycastOutput;
            if (mouseRayValid && ToolBase.RayCast(input, out raycastOutput))
            {
                if (!m_active)
                {

                        while (!Monitor.TryEnter(this.m_dataLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
                        {
                        }
                        try
                        {
                            this.m_mouseDirection = cameraDirection;
                            this.m_mousePosition = raycastOutput.m_hitPos;
                            this.m_validPosition = true;
                        }
                        finally
                        {
                            Monitor.Exit(this.m_dataLock);
                        }
                    
                }
                else
                {
                    while (!Monitor.TryEnter(this.m_dataLock, SimulationManager.SYNCHRONIZE_TIMEOUT))
                    {
                    }
                    try
                    {
                        this.m_mousePosition = raycastOutput.m_hitPos;
                        this.m_validPosition = true;
                    }
                    finally
                    {
                        Monitor.Exit(this.m_dataLock);
                    }
                }

            }
        }

        protected void BulldozeRoads()
        {
            //NetManager.instance.m_segmentGrid;
        }
        protected void BulldozeTrees()
        {
            List<uint> treesToDelete = new List<uint>();

            var minX = this.m_startPosition.x;
            var minZ = this.m_startPosition.z;
            var maxX = this.m_mousePosition.x;
            var maxZ = this.m_mousePosition.z;

            int num = Mathf.Max((int)((minX - 8f) / 32f + 270f), 0);
            int num2 = Mathf.Max((int)((minZ - 8f) / 32f + 270f), 0);
            int num3 = Mathf.Min((int)((maxX + 8f) / 32f + 270f), 539);
            int num4 = Mathf.Min((int)((maxZ + 8f) / 32f + 270f), 539);
            for (int i = num2; i <= num4; i++)
            {
                for (int j = num; j <= num3; j++)
                {
                    uint num5 = TreeManager.instance.m_treeGrid[i * 540 + j];
                    int num6 = 0;
                    while (num5 != 0u)
                    {
                        var tree = TreeManager.instance.m_trees.m_buffer[(int)((UIntPtr)num5)];
                        Vector3 position = tree.Position;
                        float num7 = Mathf.Max(Mathf.Max(minX - 8f - position.x, minZ - 8f - position.z), Mathf.Max(position.x - maxX - 8f, position.z - maxZ - 8f));
                        if (num7 < 0f)
                        {
                            
                            treesToDelete.Add(num5);
                        }
                        num5 = TreeManager.instance.m_trees.m_buffer[(int)((UIntPtr)num5)].m_nextGridTree;
                        if (++num6 >= 262144)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            foreach(uint tree in treesToDelete)
            {
                TreeManager.instance.ReleaseTree(tree);
            }
            TreeManager.instance.m_treesUpdated = true;
        }
        protected void ApplyBulldoze()
        {
            BulldozeTrees();
            BulldozeRoads();
        }

        protected override void OnToolGUI()
        {
            Event current = Event.current;
            if ( current.type == EventType.MouseDown)
            {
                if (current.button == 0)
                {
                    m_active = true ;
                    this.m_startPosition = this.m_mousePosition;
                    this.m_startDirection = Vector3.forward;
                }
            }
            else if (current.type == EventType.MouseUp)
            {
                if (current.button == 0)
                {
                    ApplyBulldoze();
                    m_active = false;
                }
            }
        }

    }
}
