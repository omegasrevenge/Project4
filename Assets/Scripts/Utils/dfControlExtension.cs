using UnityEngine;

public static class dfControlExtension
{
    public static Vector3 GetGUIScreenPos(this dfControl self)
    {
        dfControl parent = self.Parent;
        Vector3 pos = self.RelativePosition;
        while (parent != null)
        {
            pos += parent.RelativePosition;
            parent = parent.Parent;
        }
        //pos += new Vector3(self.Width / 2, self.Height / 2, 0);
        return pos;
    }

    public static void SetGUIScreenPos(this dfControl self, Vector3 pos)
    {
        dfControl parent = self.Parent;
        Vector3 offset = new Vector3();
        while (parent != null)
        {
            offset += parent.RelativePosition;
            parent = parent.Parent;
        }
        pos -= offset;
        //pos -= new Vector3(self.Width / 2, self.Height / 2, 0);
        self.RelativePosition = pos;
    }
}
