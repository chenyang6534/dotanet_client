using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
//引用的
using System.Threading;
using LitJson;

namespace cocosocket4unity
{

    public delegate bool HandleMsg(Protomsg.MsgBase d1);

    public class MsgManager
    {

        private static readonly MsgManager _instance = new MsgManager();
        public static MsgManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private MsgManager() {
            m_Lock = new Object();
        }

        Dictionary<string, HandleMsg> m_Handler = new Dictionary<string, HandleMsg>();
        public void AddListener(string key, HandleMsg handle)
        {
            if (m_Handler.ContainsKey(key))
            {
                m_Handler.Remove(key);
            }
            m_Handler.Add(key, handle);
        }
        public void RemoveListener(string key)
        {
            m_Handler.Remove(key);
        }



        List<Protomsg.MsgBase> m_Messages = new List<Protomsg.MsgBase>();
        private Object m_Lock = new Object();
        public void UpdateMessage()
        {
            while (true)
            {
                Protomsg.MsgBase msgobj = PopMessage();
                if(msgobj == null)
                {
                    return;
                }
                if( DoMessage(msgobj.MsgType, msgobj) == false)
                {
                    return;
                }
            }
        }

        public bool DoMessage(string key, Protomsg.MsgBase msg)
        {
            if (m_Handler.ContainsKey(key))
            {
                return m_Handler[key](msg);
            }
            return true;
        }

        public void AddMessage(Protomsg.MsgBase data)
        {
            lock (m_Lock)
            {
                m_Messages.Add(data);
            }
            
        }
        public Protomsg.MsgBase PopMessage()
        {
            Protomsg.MsgBase re = null;
            lock (m_Lock)
            {
                if (m_Messages.Count() > 0)
                {
                    re = m_Messages[0];
                    m_Messages.RemoveAt(0);
                }
               
            }
            return re;
        }


    }
}

