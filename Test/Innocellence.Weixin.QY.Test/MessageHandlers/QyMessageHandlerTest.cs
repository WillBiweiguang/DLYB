using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Innocellence.Weixin.Context;
using Innocellence.Weixin.QY.Entities;
using Innocellence.Weixin.QY.MessageHandlers;

namespace Innocellence.Weixin.QY.Test.MessageHandlers
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class QyMessageHandlersTest
    {
        public class CustomerMessageHandlers : QyMessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
        {
            public CustomerMessageHandlers(XDocument requestDoc, PostModel postModel, int maxRecordCount = 0)
                : base(requestDoc, postModel, maxRecordCount,true)
            {
               // this.actAfterDecryptData = AfterDecryptData;
            }

            public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
            {
                var responseMessage =
                   ResponseMessageBase.CreateFromRequestMessage(RequestMessage, ResponseMsgType.Text) as
                   ResponseMessageText;
                responseMessage.Content = "文字信息";
                return responseMessage;
            }

            public override void AfterDecryptData(string strXML, PostModel objPostModel)
            {
                //if (_isDebug)
                //{
                //    log.Debug("AfterDecryptData - " + strXML);
                //}

            }


            /// <summary>
            /// 默认消息
            /// </summary>
            /// <param name="requestMessage"></param>
            /// <returns></returns>
            public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
            {
                var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "这是一条默认消息。";
                return responseMessage;
            }
        }

        private string testXml = @"<xml><ToUserName><![CDATA[wx7618c0a6d9358622]]></ToUserName>
<Encrypt><![CDATA[h3z+AK9zKP4dYs8j1FmthAILbJghEmdo2Y1U9Pdghzann6H2KJOpepaDT1zcp09/1/e/6ta48aUXebkHlu0rhzk4GW+cvVUHzbEiQVFlIvD+q4T/NLIm8E8BM+gO+DHslM7aXmYjvgMw6AYiBx80D+nZKNyJD3I8lRT3aHCq/hez0c+HTAnZyuCi5TfUAw0c6jWSfAq61VesRw4lhV925vJUOBXT/zOw760CEsYXSr2IAr/n4aPfDgRs2Ww2h/HPiVOQ2Ms1f/BOtFiKVWMqZCxbmJ7cyPHH7+uOSAS6DtXiQAdwpEZwHz+A5QTsmK6V0C6Ifgr7zrStb7ygM7kmcrAJctPhCfG7WlfrWrFNLdtx9Q2F7d6/soinswdoYF8g56s8UWguOVkM7UFGr8H2QqrUJm5S5iFP/XNcBwvPWYA=]]></Encrypt>
<AgentID><![CDATA[2]]></AgentID>
</xml>";

        private string testXml1 = @"<xml>
   <AgentType>chat</AgentType>
   <ToUserName>CORPID</ToUserName>
   <ItemCount>2</ItemCount>
   <PackageId>3156175696255</PackageId>
   <Item>
       <FromUserName><![CDATA[fromUser]]></FromUserName>
       <CreateTime>1348831860</CreateTime>
       <MsgType><![CDATA[event]]></MsgType>
       <Event><![CDATA[update_chat]]></Event>
       <Name><![CDATA[企业应用中心]]></Name>
       <Owner><![CDATA[zhangsan]]></Owner>
       <AddUserList><![CDATA[zhaoliu]]></AddUserList>
       <DelUserList><![CDATA[lisi|wangwu]]></DelUserList>
       <ChatId><![CDATA[235364212115767297]]></ChatId>
   </Item>
   <Item>
       <FromUserName><![CDATA[fromUser]]></FromUserName>
       <CreateTime>1348831860</CreateTime>
       <MsgType><![CDATA[event]]></MsgType>
       <Event><![CDATA[create_chat]]></Event>
       <ChatInfo>
           <ChatId><![CDATA[235364212115767297]]></ChatId>
           <Name><![CDATA[企业应用中心]]></Name>
           <Owner>zhangsan</Owner>
           <UserList>zhangsan|lisi|wangwu</UserList>
       </ChatInfo>
   </Item>
<Item>
   <FromUserName><![CDATA[fromUser]]></FromUserName>
   <CreateTime>1348831860</CreateTime>
   <MsgType><![CDATA[event]]></MsgType>
   <Event><![CDATA[quit_chat]]></Event>
   <ChatId><![CDATA[235364212115767297]]></ChatId>
</Item>
<Item>
   <FromUserName><![CDATA[fromUser]]></FromUserName>
   <CreateTime>1348831860</CreateTime>
   <MsgType><![CDATA[text]]></MsgType>
   <Content><![CDATA[this is a test]]></Content>
   <MsgId>1234567890123456</MsgId>
   <Receiver>
       <Type>single</Type>
       <Id>lisi</Id>
   </Receiver>
</Item>
<Item>
   <FromUserName><![CDATA[fromUser]]></FromUserName>
   <CreateTime>1348831860</CreateTime>
   <MsgType><![CDATA[image]]></MsgType>
   <PicUrl><![CDATA[this is a url]]></PicUrl>
   <MediaId><![CDATA[media_id]]></MediaId>
   <MsgId>1234567890123456</MsgId>
   <Receiver>
       <Type>single</Type>
       <Id>lisi</Id>
   </Receiver>
</Item>
<Item>
   <FromUserName><![CDATA[fromUser]]></FromUserName>
   <CreateTime>1348831860</CreateTime>
   <MsgType><![CDATA[voice]]></MsgType>
   <MediaId><![CDATA[media_id]]></MediaId>
   <MsgId>1234567890123456</MsgId>
   <Receiver>
       <Type>single</Type>
       <Id>lisi</Id>
   </Receiver>
</Item>
</xml>";


        private string testXml2 = @"<xml>
  <ToUserName><![CDATA[wxae0c1e9b1426e718]]></ToUserName>
<Encrypt><![CDATA[p9dPHHvS9+rxg2+OxwXNapW+0KVgMuvAANu5cx1GpRgSiOz01XLs4kO8jmf6c+cUGCm+dry442EC6vZEO0x1MxB5U6/ze1R5MaXEymT3NqIIt4SlRjBfZxQ7bNpiTr9NQHtbxMJa46i9XQiHk7zztKDIXXZ+9FPxskRQNo/vY20m601Fvl8F1GmVGJ5j+kTKZkx1VTW32dP/uzuuMYmQnUYXm6uKpPwMKGr2qMfPc65Y6KotjjRHJMqfatFZwmpSLeP4XKWoxMAcrqjVPZljBGujP8SvTaP1ECsWgNhQjfIYg+kz6/Iwkb25z4x8jZc9BWNSeZJmJV6iThVhjZB2JCaxG4CExT9avJVCJ0eYUur+hNR/u4yqdg/j2DOUcSL/7/njHbcO1uW/4G2L8TwGiaIMonn2KnKNbb1d2Wtvd2zJ+DaLF485FntSYXqQ6en7NY441uixxGQ36TeGwHrI9AvBj2X4IxQpdkwXWZR5Ttbj2pQxXemx5olHno4XhhKJWEEX+Rx+9zP2UG4Zea+ROS4BFzVn6yyTVEwCFtcNXcBwXo9W+pM6Q+PP1lzrzsPzel8d2vLqrAEzoBlKBAUkUJgnvCnKHDBu69PbGYIbyida+nvAH7jIaJ62atlLBxVH]]></Encrypt>
<AgentID><![CDATA[]]></AgentID>
</xml>";


        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void TextTest()
        {
            var postModel = new PostModel()
            {
                Msg_Signature = "5a21e9fb5d7bc9c9a122046d927e8ed547e8fb30",
                Timestamp = "1460109313",
                Nonce = "209256665",

                Token = "fzBsmSaI8XE1OwBh",
                EncodingAESKey = "9J8CQ7iF9mLtQDZrUM1loOVQ6oNDxVtBi1DBU2oaewl",
                CorpId = "wxae0c1e9b1426e718"
            };
            var messageHandler = new CustomerMessageHandlers(XDocument.Parse(testXml2), postModel, 10);
            Assert.IsNotNull(messageHandler.RequestDocument);
            Assert.IsNotNull(messageHandler.RequestMessage);
            Assert.IsNotNull(messageHandler.EncryptPostData);
            Assert.IsTrue(messageHandler.AgentId == 2);

            messageHandler.Execute();

            Assert.IsNotNull(messageHandler.ResponseDocument);
            Assert.IsNotNull(messageHandler.ResponseMessage);


            Console.WriteLine(messageHandler.RequestDocument);
        }
    }
}
