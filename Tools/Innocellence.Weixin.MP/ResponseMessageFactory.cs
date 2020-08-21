﻿/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
  
    文件名：ResponseMessageFactory.cs
    文件功能描述：获取XDocument转换后的IResponseMessageBase实例
    
    
    创建标识：DLYB - 20150211
    
    修改标识：DLYB - 20150303
    修改描述：整理接口
    
    修改标识：DLYB - 20151208
    修改描述：v13.4.6 添加ConvertEntityToXml()方法
----------------------------------------------------------------*/

using System;
using System.Xml.Linq;
using DLYB.Weixin.Exceptions;
using DLYB.Weixin.MP.Entities;
using DLYB.Weixin.MP.Helpers;

namespace DLYB.Weixin.MP
{
    /// <summary>
    /// ResponseMessageFactory消息处理方法工厂类
    /// </summary>
    public static class ResponseMessageFactory
    {
        //<?xml version="1.0" encoding="utf-8"?>
        //<xml>
        //  <ToUserName><![CDATA[olPjZjsXuQPJoV0HlruZkNzKc91E]]></ToUserName>
        //  <FromUserName><![CDATA[gh_a96a4a619366]]></FromUserName>
        //  <CreateTime>63497820384</CreateTime>
        //  <MsgType>text</MsgType>
        //  <Content><![CDATA[您刚才发送了文字信息：中文
        //您还可以发送【位置】【图片】【语音】信息，查看不同格式的回复。
        //SDK官方地址：http://weixin.senparc.com]]></Content>
        //  <FuncFlag>0</FuncFlag>
        //</xml>

        /// <summary>
        /// 获取XDocument转换后的IResponseMessageBase实例（通常在反向读取日志的时候用到）。
        /// 如果MsgType不存在，抛出UnknownRequestMsgTypeException异常
        /// </summary>
        /// <returns></returns>
        public static IResponseMessageBase GetResponseEntity(XDocument doc)
        {
            ResponseMessageBase responseMessage = null;
            ResponseMsgType msgType;
            try
            {
                msgType = MsgTypeHelper.GetResponseMsgType(doc);
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMessage = new ResponseMessageText();
                        break;
                    case ResponseMsgType.Image:
                        responseMessage = new ResponseMessageImage();
                        break;
                    case ResponseMsgType.Voice:
                        responseMessage = new ResponseMessageVoice();
                        break;
                    case ResponseMsgType.Video:
                        responseMessage = new ResponseMessageVideo();
                        break;
                    case ResponseMsgType.Music:
                        responseMessage = new ResponseMessageMusic();
                        break;
                    case ResponseMsgType.News:
                        responseMessage = new ResponseMessageNews();
                        break;
					case ResponseMsgType.Transfer_Customer_Service:
						responseMessage = new ResponseMessageTransfer_Customer_Service();
						break;
                    default:
                        throw new UnknownRequestMsgTypeException(string.Format("MsgType：{0} 在ResponseMessageFactory中没有对应的处理程序！", msgType), new ArgumentOutOfRangeException());
                }
                EntityHelper.FillEntityWithXml(responseMessage, doc);
            }
            catch (ArgumentException ex)
            {
                throw new WeixinException(string.Format("ResponseMessage转换出错！可能是MsgType不存在！，XML：{0}", doc.ToString()), ex);
            }
            return responseMessage;
        }


        /// <summary>
        /// 获取XDocument转换后的IRequestMessageBase实例。
        /// 如果MsgType不存在，抛出UnknownRequestMsgTypeException异常
        /// </summary>
        /// <returns></returns>
        public static IResponseMessageBase GetResponseEntity(string xml)
        {
            return GetResponseEntity(XDocument.Parse(xml));
        }

        /// <summary>
        /// 将ResponseMessage实体转为XML
        /// </summary>
        /// <param name="entity">ResponseMessage实体</param>
        /// <returns></returns>
        public static XDocument ConvertEntityToXml(ResponseMessageBase entity)
        {
            return EntityHelper.ConvertEntityToXml(entity);
        }
    }
}
