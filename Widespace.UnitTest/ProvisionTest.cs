using System;
using NUnit.Framework;
using Widespace.AdSpaceProvisioner;
using System.Threading;
using NMock;
using System.Collections.Generic;

namespace Widespace.UnitTest
{
    [TestFixture]
    public class ProvisionTest
    {
        private MockFactory _factory = new MockFactory();
        
        /*[TestTea]
        public void Cleanup()
        {
            _factory.VerifyAllExpectationsHaveBeenMet();
            _factory.ClearExpectations();
        }*/

        [Test]
        public void EnsureOnlySingleInstanceWithThreadSafety()
        {
            int caller = 10;

            ManualResetEvent evt = new ManualResetEvent(false);
            Provisioner[] results = new Provisioner[caller];

            for (int i = 0; i < caller; i++)
            {
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    evt.WaitOne();
                    Provisioner t1Return = Provisioner.ProvisionerInstance;
                    int index = Interlocked.Decrement(ref caller);
                    results[index] = t1Return;

                });
            }

            evt.Set();
            while (caller > 0)
            {
                Thread.Sleep(0);
            }

            Provisioner expected = Provisioner.ProvisionerInstance;

            for (int i = 0; i < caller; i++)
            {
                Assert.AreEqual(expected.GetHashCode(), results[i].GetHashCode());
            }
        }

        /*[TestMethod]
        public void ShouldCallProvisionServiceMethodsToGetProvisioningInfo()
        {
            var mock = _factory.CreateMock<IHttpRequestSender>();
            //mock.Expects.One.GetProperty(_ => _.Prop).WillReturn("Hello");
            //mock.Expects.One.SetPropertyTo(_ => _.Prop = ", World");
            //IHttpRequestSender mockHttpRequestSender = Mock<IHttpRequestSender>();
            Provisioner provisioner = Provisioner.ProvisionerInstance;
            provisioner.InjectDependentInterface(mock as IHttpRequestSender);

            //using (mock.Ordered)
            //{
                mock.Expects.One.MethodWith(_ => _.GetServiceURL())
                           .Will(Return.Value("http://engine.widespace.com/map/provisioning"));

                //mock.Expects.One.MethodWith(_ => _.SendAsyncRequest("", httpRequestComleted))
                //           .Method("SendAsyncRequest")
                //           .WithAnyArguments();
                           
            //}

            provisioner.Provision();
            //mocks.VerifyAllExpectationsHaveBeenMet();
        }


        [TestMethod]
        public void ShouldUpdateStatusToInProgressWhileStartProvisioning()
        {
            Mockery mocks = new Mockery();
            IHttpRequestSender mockHttpRequestSender = mocks.NewMock<IHttpRequestSender>();
            Provisioner provisioner = Provisioner.ProvisionerInstance;
            provisioner.InjectDependentInterface(mockHttpRequestSender);

            using (mocks.Ordered)
            {
                Expect.Once.On(mockHttpRequestSender)
                           .Method("GetServiceURL")
                           .WithNoArguments()
                           .Will(Return.Value("http://engine.widespace.com/map/provisioning"));

                Expect.Once.On(mockHttpRequestSender)
                           .Method("SendAsyncRequest")
                           .WithAnyArguments();
            }

            provisioner.ResetStatus();
            Assert.AreEqual(ProvisionStatus.UNPROVISIONED, provisioner.Status);
            provisioner.Provision();
            Assert.AreEqual(ProvisionStatus.IN_PROGRESS, provisioner.Status);
            mocks.VerifyAllExpectationsHaveBeenMet();
        }


        [TestMethod]
        public void ShouldUpdateStatusToDoneAfterProvisionCompleted()
        {
            Mockery mocks = new Mockery();
            IHttpRequestSender mockProvisionService = mocks.NewMock<IHttpRequestSender>();
            Provisioner provisioner = Provisioner.ProvisionerInstance;
            provisioner.InjectDependentInterface(mockProvisionService);

            Dictionary<string, string> provisioningData = new Dictionary<string, string>();
            using (mocks.Ordered)
            {
                Expect.Once.On(mockProvisionService)
                           .Method("ParseResponse")
                           .With("{\"sdkEnabled\":true,\"sessionInfo\":{\"key\":\"YN53u9z213mEgAJ\",\"keyIndex\":0}}")
                           .Will(Return.Value(provisioningData));

                Expect.Once.On(mockProvisionService)
                           .Method("RemoveListener")
                           .WithAnyArguments();
            }

            UnitTestUtilities.RunInstanceMethod(typeof(Provisioner), "OnHttpRequestCompleted", provisioner, new object[] { "{\"sdkEnabled\":true,\"sessionInfo\":{\"key\":\"YN53u9z213mEgAJ\",\"keyIndex\":0}}" });

            mocks.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(ProvisionStatus.DONE, provisioner.Status);
        }*/

        [Test]
        public void StoreKeyWillStoreDataIntoDictionary()
        {
            var provisioner = Provisioner.ProvisionerInstance;
            provisioner.StoreKey("TestKey", "TestValue");
            Assert.AreEqual("TestValue", provisioner.ReadKey("TestKey"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void StoreKeyWillThrowExceptionIfKeyAlreadyExists()
        {
            var provisioner = Provisioner.ProvisionerInstance;
            provisioner.StoreKey("TestKey1", "TestValue1");
            provisioner.StoreKey("TestKey2", "TestValue2");
            provisioner.StoreKey("TestKey3", "TestValue3");
            provisioner.StoreKey("TestKey1", "TestValue1");
        }

        [Test]
        public void ReadKeyWillReturnExpectedValueFromStore()
        {
            var provisioner = Provisioner.ProvisionerInstance;
            provisioner.StoreKey("TestKey4", "TestValue4");
            provisioner.StoreKey("TestKey5", "TestValue5");
            Assert.AreEqual("TestValue4", provisioner.ReadKey("TestKey5"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadKeyWillThrowExceptionIfKeyNotExists()
        {
            var provisioner = Provisioner.ProvisionerInstance;
            provisioner.StoreKey("TestKey6", "TestValue7");
            provisioner.ReadKey("TestKey8");
        }

        [Test]
        public void ListenerRegisterUnregisterCheck()
        {
            var provisioner = Provisioner.ProvisionerInstance;
            provisioner.AddListener(TestDelegateMethod1);
            provisioner.AddListener(TestDelegateMethod2);
            Assert.AreEqual(2, provisioner.GetListenerCount());

            provisioner.RemoveListener(TestDelegateMethod1);
            Assert.AreEqual(1, provisioner.GetListenerCount());

            provisioner.RemoveListener(TestDelegateMethod2);
            Assert.AreEqual(0, provisioner.GetListenerCount());
        }
        
        [Test]
        [ExpectedException(typeof(TimeoutException))]
        public void HTTPRequestFailedWillThrowTimeoutException()
        {
            HttpRequestSender provisionerService = new HttpRequestSender();
            provisionerService.SendAsyncRequest("http://na.widespace.com", TestDelegateMethod3);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInvalidJsonReplyWillThrowException()
        {
            HttpRequestSender provisionerService = new HttpRequestSender();
            provisionerService.ParseResponse("InvalidJson");
        }

        [Test]
        public void ParseProvisioningResponseTest()
        {
            Provisioner provisioner = Provisioner.ProvisionerInstance;
            HttpRequestSender provisionerService = new HttpRequestSender();
            string json = "{\"sdkEnabled\":true,\"sessionInfo\":{\"key\":\"YN53u9z213mEgAJ\",\"keyIndex\":0}}";
            Dictionary<string, string> provisioningData = provisionerService.ParseResponse(json);
            string expectedKeyValue = "YN53u9z213mEgAJ";
            string actualKeyValue = string.Empty;
            provisioningData.TryGetValue("key", out actualKeyValue);
            Assert.AreEqual(expectedKeyValue, actualKeyValue);
        }

        [Test]
        public void GetServiceURLWillReturnValidUrl()
        {
            HttpRequestSender provisionerService = new HttpRequestSender();
            string url = UnitTestUtilities.RunInstanceMethod(typeof(HttpRequestSender), "GetServiceURL", provisionerService, new object[] { }).ToString();
            Assert.IsTrue(Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SendRequestFuncWillThrowExceptionforInvalidUrl()
        {
            HttpRequestSender provisionerService = new HttpRequestSender();
            provisionerService.SendAsyncRequest("bla bla bla", TestDelegateMethod3);
        }

        public void TestDelegateMethod1() { }
        public void TestDelegateMethod2() { }
        public void TestDelegateMethod3(string response) { }
    }
}
