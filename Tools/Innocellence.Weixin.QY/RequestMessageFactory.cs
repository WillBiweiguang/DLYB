﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
  
    文件名：RequestMessageFactory.cs
    文件功能描述：获取XDocument转换后的IRequestMessageBase实例
    
    
    创建标识：DLYB - 20150313
    
    修改标识：DLYB - 20150313
    修改描述：整理接口
    
    修改标识：DLYB - 20150507
    修改描述：添加 事件 异步任务完成事件推送
----------------------------------------------------------------*/

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.QY.Entities;
using DLYB.Weixin.QY.Helpers;

namespace DLYB.Weixin.QY
{
    public static class RequestMessageFactory
    {
        /// <summary>
        /// 获取XDocument转换后的IRequestMessageBase实例。
        /// 如果MsgType不存在，抛出UnknownRequestMsgTypeException异常
        /// </summary>
        /// <returns></returns>
        public static IRequestMessageBase GetRequestEntity(XDocument doc)
        {
            RequestMessageBase requestMessage = null;
            RequestMsgType msgType;
            ThirdPartyInfo infoType;

            var o=doc.Root.Element("AgentType");
            if (o != null)
            {
               return GetRequestChatEntity(doc, o.Value);
            }



            //区分普通消息与第三方应用授权推送消息，MsgType有值说明是普通消息，反之则是第三方应用授权推送消息
            if (doc.Root.Element("MsgType") != null)
            {
                //常规推送信息
                try
                {
                    msgType = MsgTypeHelper.GetRequestMsgType(doc);

                    switch (msgType)
                    {
                        case RequestMsgType.Text:
                            requestMessage = new RequestMessageText();
                            break;
                        case RequestMsgType.Location:
                            requestMessage = new RequestMessageLocation();
                            break;
                        case RequestMsgType.Image:
                            requestMessage = new RequestMessageImage();
                            break;
                        case RequestMsgType.Voice:
                            requestMessage = new RequestMessageVoice();
                            break;
                        case RequestMsgType.Video:
                            requestMessage = new RequestMessageVideo();
                            break;
                        case RequestMsgType.ShortVideo:
                            requestMessage = new RequestMessageShortVideo();
                            break;
                        case RequestMsgType.Event:
                            //判断Event类型
                            switch (doc.Root.Element("Event").Value.ToUpper())
                            {
                                case "CLICK"://菜单点击
                                    requestMessage = new RequestMessageEvent_Click();
                                    break;
                                case "VIEW"://URL跳转
                                    requestMessage = new RequestMessageEvent_View();
                                    break;
                                case "SUBSCRIBE"://订阅（关注）
                                    requestMessage = new RequestMessageEvent_Subscribe();
                                    break;
                                case "UNSUBSCRIBE"://取消订阅（关注）
                                    requestMessage = new RequestMessageEvent_UnSubscribe();
                                    break;
                                case "SCANCODE_PUSH"://扫码推事件(scancode_push)
                                    requestMessage = new RequestMessageEvent_Scancode_Push();
                                    break;
                                case "SCANCODE_WAITMSG"://扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
                                    requestMessage = new RequestMessageEvent_Scancode_Waitmsg();
                                    break;
                                case "PIC_SYSPHOTO"://弹出系统拍照发图(pic_sysphoto)
                                    requestMessage = new RequestMessageEvent_Pic_Sysphoto();
                                    break;
                                case "PIC_PHOTO_OR_ALBUM"://弹出拍照或者相册发图（pic_photo_or_album）
                                    requestMessage = new RequestMessageEvent_Pic_Photo_Or_Album();
                                    break;
                                case "PIC_WEIXIN"://弹出微信相册发图器(pic_weixin)
                                    requestMessage = new RequestMessageEvent_Pic_Weixin();
                                    break;
                                case "LOCATION_SELECT"://弹出地理位置选择器（location_select）
                                    requestMessage = new RequestMessageEvent_Location_Select();
                                    break;
                                case "LOCATION"://上报地理位置事件（location）
                                    requestMessage = new RequestMessageEvent_Location();
                                    break;
                                case "ENTER_AGENT"://用户进入应用的事件推送（enter_agent）
                                    requestMessage = new RequestMessageEvent_Enter_Agent();
                                    break;
                                case "BATCH_JOB_RESULT"://异步任务完成事件推送(batch_job_result)
                                    requestMessage = new RequestMessageEvent_Batch_Job_Result();
                                    break;
                                default://其他意外类型（也可以选择抛出异常）
                                    requestMessage = new RequestMessageEventBase();
                                    break;
                            }
                            break;
                        default:
                            throw new UnknownRequestMsgTypeException(string.Format("MsgType：{0} 在RequestMessageFactory中没有对应的处理程序！", msgType), new ArgumentOutOfRangeException());//为了能够对类型变动最大程度容错（如微信目前还可以对公众账号suscribe等未知类型，但API没有开放），建议在使用的时候catch这个异常
                    }
                    EntityHelper.FillEntityWithXml(requestMessage, doc);
                }
                catch (ArgumentException ex)
                {
                    throw new WeixinException(string.Format("RequestMessage转换出错！可能是MsgType不存在！，XML：{0}", doc.ToString()), ex);
                }
            }
            else if (doc.Root.Element("InfoType") != null)
            {
                //第三方回调
                try
                {
                    infoType = MsgTypeHelper.GetThirdPartyInfo(doc);
                    switch (infoType)
                    {
                        case ThirdPartyInfo.SUITE_TICKET://推送suite_ticket协议
                            requestMessage = new RequestMessageInfo_Suite_Ticket();
                            break;
                        case ThirdPartyInfo.CHANGE_AUTH://变更授权的通知
                            requestMessage = new RequestMessageInfo_Change_Auth();
                            break;
                        case ThirdPartyInfo.CANCEL_AUTH://取消授权的通知
                            requestMessage = new RequestMessageInfo_Cancel_Auth();
                            break;
                        default:
                            throw new UnknownRequestMsgTypeException(string.Format("InfoType：{0} 在RequestMessageFactory中没有对应的处理程序！", infoType), new ArgumentOutOfRangeException());//为了能够对类型变动最大程度容错（如微信目前还可以对公众账号suscribe等未知类型，但API没有开放），建议在使用的时候catch这个异常
                    }
                    EntityHelper.FillEntityWithXml(requestMessage, doc);
                }
                catch (ArgumentException ex)
                {
                    throw new WeixinException(string.Format("RequestMessage转换出错！可能是MsgType和InfoType都不存在！，XML：{0}", doc.ToString()), ex);
                }
            }
            else
            {
                throw new WeixinException(string.Format("RequestMessage转换出错！可能是MsgType和InfoType都不存在！，XML：{0}", doc.ToString()));
            }
            
            return requestMessage;
        }


        public static IRequestMessageBase GetRequestChatEntity(XDocument xml, string strAgentType)
        {
            RequestMessageChat requestMessage = new RequestMessageChat();

            var oAgentType = MsgTypeHelper.GetResponseAgentType(strAgentType);
            //switch (oAgentType)
            //{

            //    case AgentType.chat:
            //    case AgentType.kf_internal:
            //        EntityHelper.FillEntityWithXml(requestMessage, xml);
            //        break;
            //}

            EntityHelper.FillEntityWithXml(requestMessage, xml);

            //return GetRequestEntity(XDocument.Parse(xml));

            return requestMessage;
        }


        /// <summary>
        /// 获取XDocument转换后的IRequestMessageBase实例。
        /// 如果MsgType不存在，抛出UnknownRequestMsgTypeException异常
        /// </summary>
        /// <returns></returns>
        public static IRequestMessageBase GetRequestEntity(string xml)
        {
            return GetRequestEntity(XDocument.Parse(xml));
        }

        /// <summary>
        /// 获取XDocument转换后的IRequestMessageBase实例。
        /// 如果MsgType不存在，抛出UnknownRequestMsgTypeException异常
        /// </summary>
        /// <param name="stream">如Request.InputStream</param>
        /// <returns></returns>
        public static IRequestMessageBase GetRequestEntity(Stream stream)
        {
            using (XmlReader xr = XmlReader.Create(stream))
            {
                var doc = XDocument.Load(xr);
                return GetRequestEntity(doc);
            }
        }

        /// <summary>
        /// 获取微信服务器发送过来的加密xml信息
        /// </summary>
        /// <param name="xml"></param>
        public static EncryptPostData GetEncryptPostData(string xml)
        {
            if (xml == null)
            {
                return null;
            }
            var encryptPostData = new EncryptPostData();
            encryptPostData.FillEntityWithXml(XDocument.Parse(xml));
            return encryptPostData;
        }
    }
}
