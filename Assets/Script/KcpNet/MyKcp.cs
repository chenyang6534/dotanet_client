using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
//引用的
using System.Threading;
using LitJson;
using Google.Protobuf;
using UnityEngine;

namespace cocosocket4unity
{
    public class Person
    {
        // C# 3.0 auto-implemented properties
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class MyKcp : KcpClient
	{
        public static int PingValue = 0;
        protected double m_SendHeartTime = 0;
        private static MyKcp _instance = new MyKcp();
        public static MyKcp Instance
        {
            get
            {
                return _instance;
            }
        }

        private MyKcp() { }
        


        protected override void HandleReceive(ByteBuf bb)
        {

            IMessage IMperson = new Protomsg.MsgBase();
            Protomsg.MsgBase p1 = (Protomsg.MsgBase)IMperson.Descriptor.Parser.ParseFrom(bb.GetRaw());

            if( p1.MsgType == "SC_Heart")
            {
                UnityEngine.Debug.Log("heart");
                //this.SendHeartMsg();
                var time = Tool.GetTime();
                PingValue = (int)Math.Floor((time - m_SendHeartTime) * 1000);
                UnityEngine.Debug.Log("ping:"+ PingValue);
                return;
            }



           MsgManager.Instance.AddMessage(p1);
          //  UnityEngine.Debug.Log("MsgType:" + p1.MsgType+ "ModeType:" + p1.ModeType + "ConnectId:" + p1.ConnectId + "Uid:" + p1.Uid);
            //this.Send(bb.Copy());
        }
        /// <summary>
        /// 异常
        /// </summary>
        protected override void HandleException(Exception ex)
        {
            base.HandleException(ex);
            UnityEngine.Debug.Log("HandleException"+ex.ToString());
            UnityEngine.Debug.Log("HandleException:" + Tool.getMemory(this));
            Protomsg.CC_Disconnect msg = new Protomsg.CC_Disconnect();
            msg.Err = ex.ToString();

            Protomsg.MsgBase msg1 = new Protomsg.MsgBase();
            msg1.ModeType = "";
            msg1.MsgType = "CC_Disconnect";
            if (msg != null)
            {
                msg1.Datas = ByteString.CopyFrom(msg.ToByteArray());
            }
            MsgManager.Instance.AddMessage(msg1);

        }
        /// <summary>
        /// 超时
        /// </summary>
        protected override void HandleTimeout()
        {
            base.HandleTimeout();
            UnityEngine.Debug.Log("HandleTimeout");

            Protomsg.MsgBase msg = new Protomsg.MsgBase();
            msg.MsgType = "TimeOut";
            MsgManager.Instance.AddMessage(msg);


        }
        public void Destroy()
        {
            this.Stop();
            Thread.Sleep(3000);
        }


        public void Create(String ip,int port)
        {
            //this.Stop();
            //Thread.Sleep(1000);
            _instance = new MyKcp();

            this.NoDelay(1, 10, 2, 1);//fast
            this.WndSize(128, 128);
            this.Timeout(10 * 1000);
            //client.SetMtu(512);
            this.SetMinRto(10);
            //client.SetConv(121106);
            UnityEngine.Debug.Log("Create");
            this.Connect(ip, port);
            this.Start();

            //this.SendHeartMsg();

            this.DoHeart();

        }

        


        public void SendMsg(Protomsg.MsgBase msg)
        {
            
            //UnityEngine.Debug.Log("msg:"+ msg.ToString());

            ByteBuf bb = new ByteBuf(msg.ToByteArray());

            this.Send(bb);

        }

        public void SendMsg(String modetype,String msgtype, IMessage msg)
        {
            Protomsg.MsgBase msg1 = new Protomsg.MsgBase();
            msg1.ModeType = modetype;
            msg1.MsgType = msgtype;
            if ( msg != null)
            {
                msg1.Datas = ByteString.CopyFrom(msg.ToByteArray());
            }
            

            ByteBuf bb = new ByteBuf(msg1.ToByteArray());

            this.Send(bb);

        }

        //处理心跳
        public void DoHeart()
        {
            Thread t = new Thread(new ThreadStart(HeartThread));//心跳
            t.IsBackground = true;
            t.Start();
        }

        public void HeartThread()
        {
            while (this.running)
            {
                UnityEngine.Debug.Log("111HeartThread:" + Tool.getMemory(this));
                this.SendHeartMsg();
                Thread.Sleep(3000);
            }
        }

        public void SendHeartMsg()
        {
            Protomsg.MsgBase msg1 = new Protomsg.MsgBase();
            msg1.MsgType = "CS_Heart";
            ByteBuf bb = new ByteBuf(msg1.ToByteArray());
            UnityEngine.Debug.Log("SendHeartMsg:"+ Tool.getMemory(this));
            this.Send(bb);

            m_SendHeartTime = Tool.GetTime();

        }


    }
}

