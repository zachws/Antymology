using Antymology.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Antymology.UI
{
    public class UITerrainEditor : Singleton<UITerrainEditor>
    {
        private AbstractBlock currentBlockType = new StoneBlock();

        void ReplaceBlockCursor()
        {
            //Replaces the block specified where the mouse cursor is pointing

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ReplaceBlockAt(hit);
            }
        }

        void AddBlockCursor()
        {
            //Adds the block specified where the mouse cursor is pointing

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                AddBlockAt(hit);
            }
        }

        void ReplaceBlockAt(RaycastHit hit)
        {
            Vector3 position = hit.point;
            position += (hit.normal * -0.5f);
            SetBlockAt(position);
        }

        void AddBlockAt(RaycastHit hit)
        {
            //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
            Vector3 position = hit.point;
            position += (hit.normal * 0.5f);

            SetBlockAt(position);

        }

        void SetBlockAt(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            int z = Mathf.RoundToInt(position.z);
            SetBlockAt(x, y, z);
        }

        void SetBlockAt(int x, int y, int z)
        {
            Debug.Log(WorldManager.Instance.GetBlock(x, y, z));
            WorldManager.Instance.SetBlock(x, y, z, currentBlockType);
            Debug.Log(WorldManager.Instance.GetBlock(x, y, z));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentBlockType = new AcidicBlock();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentBlockType = new ContainerBlock();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentBlockType = new GrassBlock();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentBlockType = new MulchBlock();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentBlockType = new StoneBlock();
            }
            if (Input.GetMouseButtonDown(0))
            {
                AddBlockCursor();
            }
            if (Input.GetMouseButtonDown(1))
            {
                AbstractBlock oldBlockType = currentBlockType;
                currentBlockType = new AirBlock();
                ReplaceBlockCursor();
                currentBlockType = oldBlockType;
            }
        }

    }
}
