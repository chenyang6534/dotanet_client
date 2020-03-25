using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class ExchangeInfo
{
    private GComponent main;
    
    public ExchangeInfo()
    {
        MsgManager.Instance.AddListener("SC_GetExchangeShortCommoditys", new HandleMsg(this.SC_GetExchangeShortCommoditys));
        MsgManager.Instance.AddListener("SC_GetExchangeDetailedCommoditys", new HandleMsg(this.SC_GetExchangeDetailedCommoditys));
        MsgManager.Instance.AddListener("SC_GetSellUIInfo", new HandleMsg(this.SC_GetSellUIInfo));
        
        //获取数据
        Protomsg.CS_GetExchangeShortCommoditys msg1 = new Protomsg.CS_GetExchangeShortCommoditys();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetExchangeShortCommoditys", msg1);
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);

        main = UIPackage.CreateObject("GameUI", "exchange").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f,0.5f);

        main.GetController("page").onChanged.Add(() => {
            var curpagename = main.GetController("page").selectedPage;
            if (curpagename == "buy")
            {
                //获取数据
                Debug.Log("CS_GetExchangeShortCommoditys");
                Protomsg.CS_GetExchangeShortCommoditys msg2 = new Protomsg.CS_GetExchangeShortCommoditys();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetExchangeShortCommoditys", msg2);
            }
            else
            {
                //获取数据
                Debug.Log("CS_GetSellUIInfo");
                Protomsg.CS_GetSellUIInfo msg2 = new Protomsg.CS_GetSellUIInfo();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetSellUIInfo", msg2);
            }
        });

        main.GetChild("buy").asButton.onClick.Add(() =>
        {
            //获取数据
            Debug.Log("CS_GetExchangeShortCommoditys");
            Protomsg.CS_GetExchangeShortCommoditys msg2 = new Protomsg.CS_GetExchangeShortCommoditys();
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetExchangeShortCommoditys", msg2);
        });

        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });
    }
    public bool SC_GetExchangeShortCommoditys(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetExchangeShortCommoditys:");
        IMessage IMperson = new Protomsg.SC_GetExchangeShortCommoditys();
        Protomsg.SC_GetExchangeShortCommoditys p1 = (Protomsg.SC_GetExchangeShortCommoditys)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("buylist").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.ExchangeShortCommodityData[] allplayer = new Protomsg.ExchangeShortCommodityData[p1.Commoditys.Count];
        int index = 0;
        foreach (var item in p1.Commoditys)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer, (a, b) => {

            if(a.ItemID > b.ItemID)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        });
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.ItemID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "exchangecommodity_short").asCom;
            onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(item.ItemID);
            });
            onedropitem.GetChild("price").asTextField.text = item.SellCount + "";
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.onClick.Add(() =>
            {
                //获取数据
                Protomsg.CS_GetExchangeDetailedCommoditys msg1 = new Protomsg.CS_GetExchangeDetailedCommoditys();
                msg1.ItemID = item.ItemID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetExchangeDetailedCommoditys", msg1);
                
            });
            main.GetChild("buylist").asList.AddChild(onedropitem);
        }

        return true;
    }
    public bool SC_GetExchangeDetailedCommoditys(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetExchangeDetailedCommoditys:");
        IMessage IMperson = new Protomsg.SC_GetExchangeDetailedCommoditys();
        Protomsg.SC_GetExchangeDetailedCommoditys p1 = (Protomsg.SC_GetExchangeDetailedCommoditys)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("buylist").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.ExchangeDetailedCommodityData[] allplayer = new Protomsg.ExchangeDetailedCommodityData[p1.Commoditys.Count];
        int index = 0;
        foreach (var item in p1.Commoditys)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer, (a, b) => {

            if (a.CommodityData.Price > b.CommodityData.Price)
            {
                return 1;
            }
            else if (a.CommodityData.Price == b.CommodityData.Price)
            {
                if(a.CommodityData.TypeID > b.CommodityData.TypeID)
                {
                    return 1;
                }
                else if (a.CommodityData.TypeID < b.CommodityData.TypeID)
                {
                    return -1;
                }
                return 0;
            }
            else
            {
                return -1;
            }
        });
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.CommodityData.ItemID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "exchangecommodity").asCom;
            onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            onedropitem.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(item.CommodityData.ItemID);
            });
            onedropitem.GetChild("price").asTextField.text = item.CommodityData.Price + "";
            onedropitem.GetChild("level").asTextField.text = item.CommodityData.Level + "";
            onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.CommodityData.PriceType);
            onedropitem.GetChild("n2").onClick.Add(() =>
            {
                //购买
                var sellwindow = UIPackage.CreateObject("GameUI", "exchangebuy").asCom;
                GRoot.inst.AddChild(sellwindow);
                sellwindow.xy = Tool.GetPosition(0.5f, 0.5f);
                sellwindow.GetChild("close").onClick.Add(() =>
                {
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("yes_btn").onClick.Add(() =>
                {
                    Protomsg.CS_BuyExchangeCommodity msg1 = new Protomsg.CS_BuyExchangeCommodity();
                    msg1.ID = item.CommodityData.TypeID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_BuyExchangeCommodity", msg1);
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
                sellwindow.GetChild("item").asCom.GetChild("level").asTextField.text = item.CommodityData.Level + "";
                sellwindow.GetChild("name").asTextField.text = clientitem.Name;
                sellwindow.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.CommodityData.PriceType);
                sellwindow.GetChild("price").asTextField.text = item.CommodityData.Price + "";
                

            });
            main.GetChild("buylist").asList.AddChild(onedropitem);
        }
        return true;
    }

    //

    

    public bool SC_GetSellUIInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetSellUIInfo:");
        IMessage IMperson = new Protomsg.SC_GetSellUIInfo();
        Protomsg.SC_GetSellUIInfo p1 = (Protomsg.SC_GetSellUIInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("sellablelist").asList.RemoveChildren(0, -1, true);
        //------------------可以售卖的道具----------------------
        //处理排序
        Protomsg.UnitEquip[] allplayer = new Protomsg.UnitEquip[p1.Equips.Count];
        int index = 0;
        foreach (var item in p1.Equips)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer, (a, b) => {

            if(a.Pos > b.Pos)
            {
                return 1;
            }
            else if (a.Pos < b.Pos)
            {
                return -1;
            }
            return 0;
        });
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.TypdID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
            onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("icon").onClick.Add(() =>
            {
                //售卖
                var sellwindow = UIPackage.CreateObject("GameUI", "Shelf1").asCom;
                GRoot.inst.AddChild(sellwindow);
                sellwindow.xy = Tool.GetPosition(0.5f, 0.5f);
                sellwindow.GetChild("close").onClick.Add(() =>
                {
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("yes_btn").onClick.Add(() =>
                {
                    var txt = sellwindow.GetChild("input").asTextInput.text;
                    if (txt.Length <= 0)
                    {
                        Tool.NoticeWords("请输入价格！", null);
                        return;
                    }
                    
                    int price = 0;
                    try
                    {
                        price = Convert.ToInt32(txt); //报异常
                    }
                    catch (SystemException e)
                    {
                        Tool.NoticeWords("请输入正确的价格！", null);
                        return;
                    }
                    if(price <= 0)
                    {
                        Tool.NoticeWords("请输入正确的价格！", null);
                        return;
                    }
                    //上架
                    Protomsg.CS_ShelfExchangeCommodity msg1 = new Protomsg.CS_ShelfExchangeCommodity();
                    msg1.BagPos = item.Pos;
                    msg1.PriceType = 10001;
                    msg1.Price = price;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ShelfExchangeCommodity", msg1);
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
                sellwindow.GetChild("item").asCom.GetChild("level").asTextField.text = item.Level + "";
                sellwindow.GetChild("name").asTextField.text = clientitem.Name;
                sellwindow.GetChild("pricetype_shouxufei").asLoader.url = Tool.GetPriceTypeIcon(p1.ShelfExchangeFeePriceType);
                sellwindow.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(10001);
                sellwindow.GetChild("price_shouxufei").asTextField.text = p1.ShelfExchangeFeePrice+"";
                sellwindow.GetChild("unshelf_time").asTextField.SetVar("p1", Tool.Time2String(p1.AutoUnShelfTime));
                sellwindow.GetChild("unshelf_time").asTextField.FlushVars();

            });
            onedropitem.GetChild("level").asTextField.text = item.Level + "";
            main.GetChild("sellablelist").asList.AddChild(onedropitem);
        }

        //------------------正在售卖的道具----------------------
        main.GetChild("sellinglist").asList.RemoveChildren(0, -1, true);
        //处理排序
        Protomsg.ExchangeDetailedCommodityData[] allselling = new Protomsg.ExchangeDetailedCommodityData[p1.Commoditys.Count];
        index = 0;
        foreach (var item in p1.Commoditys)
        {
            allselling[index++] = item;
        }
        System.Array.Sort(allselling, (a, b) => {
            Debug.Log("a:" + a.CommodityData.TypeID + "  b:" + b.CommodityData.TypeID);
            if (a.CommodityData.TypeID > b.CommodityData.TypeID)
            {
                return 1;
            }else if(a.CommodityData.TypeID < b.CommodityData.TypeID)
            {
                return -1;
            }
            return 0;
        });
        foreach (var item in allselling)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.CommodityData.ItemID);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "selling").asCom;
            onedropitem.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("icon").onClick.Add(() =>
            {
                new ItemInfo(item.CommodityData.ItemID);
                
            });
            onedropitem.onClick.Add(() =>
            {
                //下架
                var sellwindow = UIPackage.CreateObject("GameUI", "exchangeunshelf").asCom;
                GRoot.inst.AddChild(sellwindow);
                sellwindow.xy = Tool.GetPosition(0.5f, 0.5f);
                sellwindow.GetChild("close").onClick.Add(() =>
                {
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("yes_btn").onClick.Add(() =>
                {
                    Protomsg.CS_UnShelfExchangeCommodity msg1 = new Protomsg.CS_UnShelfExchangeCommodity();
                    msg1.ID = item.CommodityData.TypeID;
                    MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_UnShelfExchangeCommodity", msg1);
                    sellwindow.Dispose();
                });
                sellwindow.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
                sellwindow.GetChild("item").asCom.GetChild("level").asTextField.text = item.CommodityData.Level + "";
                sellwindow.GetChild("name").asTextField.text = clientitem.Name;
                sellwindow.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.CommodityData.PriceType);
                sellwindow.GetChild("price").asTextField.text = item.CommodityData.Price + "";
            });
            onedropitem.GetChild("price").asTextField.text = item.CommodityData.Price + "";
            onedropitem.GetChild("level").asTextField.text = item.CommodityData.Level + "";
            onedropitem.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(item.CommodityData.PriceType);
            onedropitem.GetChild("name").asTextField.text = clientitem.Name;
            main.GetChild("sellinglist").asList.AddChild(onedropitem);
        }
        

        //----提示描述--
        main.GetChild("maxcount").asTextField.text = p1.Commoditys.Count + "/" + p1.ShelfExchangeLimit;
        main.GetChild("Tax").asTextField.SetVar("p1", (p1.SellExchangeTax * 100).ToString("0.00") + "%");
        main.GetChild("Tax").asTextField.FlushVars();
        return true;
    }
    

    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetExchangeShortCommoditys");
        MsgManager.Instance.RemoveListener("SC_GetExchangeDetailedCommoditys"); 
        MsgManager.Instance.RemoveListener("SC_GetSellUIInfo");
        
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
