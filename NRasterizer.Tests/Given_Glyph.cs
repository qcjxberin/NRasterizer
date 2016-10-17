﻿using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using NRasterizer.Rasterizer;
using Moq;

namespace NRasterizer.Tests
{
    [TestFixture]
    public class Given_Glyph
    {
        private Renderer _renderer;
        private Mock<IGlyphRasterizer> _mock;
        private MockSequence _sequence;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<IGlyphRasterizer>(MockBehavior.Strict);
            _sequence = new MockSequence();
            _mock.InSequence(_sequence).Setup(r => r.BeginRead(1));
            _renderer = new Renderer(null, _mock.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mock.VerifyAll();
        }

        private void Render(Glyph glyph)
        {
            _renderer.RenderGlyph(0, 0, glyph);
        }

        private void StartAt(double x, double y)
        {   
            _mock.InSequence(_sequence).Setup(d => d.MoveTo(x, y));
        }

        private void AssertLineTo(double x, double y)
        {
            _mock.InSequence(_sequence).Setup(d => d.LineTo(x, y));
        }

        private void AssertBezierTo(double cx, double cy, double endx, double endy)
        {
            _mock.InSequence(_sequence).Setup(d => d.Curve3(cx, cy, endx, endy));
        }

        private void AssertContourDone()
        {
            //_mock.InSequence(_sequence).Setup(d => d.CloseFigure()).Callback(() => System.Console.WriteLine("called"));
            _mock.Setup(d => d.CloseFigure());
            _mock.Setup(d => d.EndRead());
        }

        [Test]
        public void With_Four_Line_Countour()
        {
            var x = new short[] { 0, 128, 128, 0 };
            var y = new short[] { 0, 0, 128, 128 };
            var on = new bool[] { true, true, true, true };

            // These coordinates ssytems are flipped in y-direction by EM-square baseline (2048) and then scaled by 1/64
            StartAt(0, 32);
            AssertLineTo(2, 32);
            AssertLineTo(2, 30);
            AssertLineTo(0, 30);
            AssertLineTo(0, 32);
            AssertContourDone();

            Render(new Glyph(x, y, on, new ushort[] { 3 }, null));
        }

        [Test]
        public void With_Line_And_Bezier_Countour()
        {
            var x = new short[] { 0, 128, 128, 0 };
            var y = new short[] { 0, 0, 128, 128 };
            var on = new bool[] { true, true, false, true };

            // These coordinates ssytems are flipped in y-direction by EM-square baseline (2048) and then scaled by 1/64
            StartAt(0, 32);
            AssertLineTo(2, 32);
            AssertBezierTo(2, 30, 0, 30);
            AssertLineTo(0, 32);
            AssertContourDone();

            Render(new Glyph(x, y, on, new ushort[] { 3 }, null));
        }
    }
}
