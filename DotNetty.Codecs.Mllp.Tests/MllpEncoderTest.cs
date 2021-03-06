﻿using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Xunit;

namespace DotNetty.Codecs.Mllp.Tests
{
    public class MllpEncoderTest
    {
        public MllpEncoderTest()
        {
            var iso = Encoding.GetEncoding("ISO-8859-1");
            var charBytes = iso.GetBytes("A");
            _msg = Unpooled.CopiedBuffer(charBytes);
        }

        private readonly IByteBuffer _msg;
        private readonly byte[] _prepend = {11};
        private readonly byte[] _append = {28, 13};

        [Fact]
        public void TestEncodedLength()
        {
            var ch = new EmbeddedChannel(new FrameEncoder(_prepend, _append));
            for (var i = 0; i < 2; i++)
            {
                ch.WriteOutbound(_msg.Retain());

                var prepend = ch.ReadOutbound<byte[]>();
                Assert.Equal(_prepend.Length, prepend.Length);


                var buf = ch.ReadOutbound<IByteBuffer>();
                Assert.Same(buf, _msg);
                buf.Release();

                var append = ch.ReadOutbound<byte[]>();
                Assert.Equal(_append.Length, append.Length);
                Assert.Equal(_append, append);
            }
        }
    }
}