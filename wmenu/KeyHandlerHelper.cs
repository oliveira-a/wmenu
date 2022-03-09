using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace wmenu
{
    /// <summary>
    /// For more info on how to handle key presses globally see this comment by Curtis Rutland
    /// https://www.dreamincode.net/forums/topic/180436-global-hotkeys/ 
    /// </summary>
    public class KeyHandler
    {
        public const int WM_HOTKEY_MS_ID = 0x0312;

        private const int WIN_KEY = 0x0008;
        private int _key, _id;
        private IntPtr _hWnd;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]

        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        public override int GetHashCode()
        {
            return WIN_KEY ^ _key ^ _hWnd.ToInt32();
        }

        public bool Register()
        {
            return RegisterHotKey(_hWnd, _id, WIN_KEY, _key);
        }

        public bool Unregister()
        {
            return UnregisterHotKey(_hWnd, _id);
        }

        public KeyHandler(Keys key, Form form)
        {
            _key = (int)key;
            _hWnd = form.Handle;
            _id = this.GetHashCode();
        }
    }
}
