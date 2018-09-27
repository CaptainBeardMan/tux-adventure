using UnityEngine;
using System.Collections;

public class Door
{
        private int levelDest;
        private float[] levelB_map;
        private int[] levelB_grid;

        public Door(int b, float bx, float by, int bgx, int bgy)
        {
            levelDest = b;
            levelB_map = new float[] { bx, by };
            levelB_grid = new int[] { bgx, bgy };
        }

        public int getDestinationLevel() { return levelDest; }
        public float getDestinationMapx() { return levelB_map[0]; }
        public float getDestinationMapy() { return levelB_map[1]; }
        public int getDestinationGridX() { return levelB_grid[0]; }
        public int getDestinationGridY() { return levelB_grid[1]; }


}
