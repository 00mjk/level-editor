using System;
using System.IO;
using System.Text;
using LayerDataReaderWriter.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LayerDataReaderWriter.UnitTests
{
    [TestClass]
    public class UnitTestV1
    {
        [TestMethod]
        public void TestWriter01()
        {
            var writer = new WriterV1();

            var success = true;
            try
            {
                writer.Write(null, null);
                success = false;
            }
            catch
            {
            }

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestWriter02()
        {
            var writer = new WriterV1();
            Assert.AreEqual(writer.Version, 1u);
        }

        [TestMethod]
        public void TestWriter03()
        {
            var stream = new MemoryStream();

            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 10.0f, 90.0f, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03),
                    }),
                new LayerBlock(1, 2, 100.0f, 900.0f, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable),
                    }),
            };

            var writer = new WriterV1();
            writer.Write(layerBlocks, new BinaryWriter(stream, Encoding.UTF8));

            stream.Position = 0;

            var sr = new BinaryReader(stream);

            Assert.AreEqual(sr.ReadByte(), 2); // block count

            Assert.AreEqual(sr.ReadByte(), 0); // block id
            Assert.AreEqual(sr.ReadByte(), 1); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 10.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 90.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 1); // block flags (take off)
            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv01); // type

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv02); // type

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv03); // type

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 100.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 900.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 0); // block flags (take off)
            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemBlue); // type

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemPurple); // type

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleBreakable); // type

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestWriter04()
        {
            uint version = 1;

            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 20.0f, 180.0f, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv03),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.GemRed),
                    }),
                new LayerBlock(1, 2, 200.0f, 1800.0f, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.ObstacleBreakable),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.ObstacleStatic),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv01),
                    }),
            };

            var stream = new MemoryStream();
            ReaderWriterManager.Write(layerBlocks, stream, Encoding.UTF8, version);

            stream.Position = 0;

            var sr = new BinaryReader(stream);

            Assert.AreEqual(sr.ReadUInt32(), ReaderWriterManager.MagicNumber);
            Assert.AreEqual(sr.ReadUInt32(), version);

            Assert.AreEqual(sr.ReadByte(), 2); // block count

            Assert.AreEqual(sr.ReadByte(), 0); // block id
            Assert.AreEqual(sr.ReadByte(), 1); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 20.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 180.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 0); // block flags (take off)
            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv03); // type

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemPurple); // type

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemRed); // type

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 200.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 1800.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 1); // block flags (take off)
            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleBreakable); // type

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleStatic); // type

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv01); // type

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestReader01()
        {
            var reader = new ReaderV1();

            var success = true;
            try
            {
                reader.Read(null);
                success = false;
            }
            catch
            {
            }

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestReader02()
        {
            var reader = new ReaderV1();
            Assert.AreEqual(reader.Version, 1u);
        }

        [TestMethod]
        public void TestReader03()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 30.0f, 270.0f, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03),
                    }),
                new LayerBlock(1, 2, 300.0f, 2700.0f, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable),
                    }),
            };

            var stream = new MemoryStream();
            ReaderWriterManager.Write(layerBlocks1, stream, Encoding.UTF8, 1);

            stream.Position = 0;

            var layerBlocks2 = ReaderWriterManager.Read(stream, Encoding.UTF8) as LayerBlock[];
            Assert.IsNotNull(layerBlocks2);

            Assert.AreEqual(layerBlocks1.Length, layerBlocks2.Length); // block count

            for (int b = 0; b < 2; b++)
            {
                Assert.AreEqual(layerBlocks1[b].Identifier, layerBlocks2[b].Identifier); // block id
                Assert.AreEqual(layerBlocks1[b].Difficulty, layerBlocks2[b].Difficulty); // block difficulty
                Assert.AreEqual(layerBlocks1[b].Width, layerBlocks2[b].Width); // block width
                Assert.AreEqual(layerBlocks1[b].Height, layerBlocks2[b].Height); // block height
                Assert.AreEqual(layerBlocks1[b].IsTakeOffBlock, layerBlocks2[b].IsTakeOffBlock); // block flags (take off)

                layerBlocks2[b].ReadData();
                Assert.IsNotNull(layerBlocks2[b].Elements);

                Assert.AreEqual(layerBlocks1[b].Elements.Length, layerBlocks2[b].Elements.Length); // block element count

                for (int e = 0; e < 3; e++)
                {
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Tx, layerBlocks2[b].Elements[e].Tx); // tx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Ty, layerBlocks2[b].Elements[e].Ty); // ty
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Angle, layerBlocks2[b].Elements[e].Angle); // angle
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sx, layerBlocks2[b].Elements[e].Sx); // sx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sy, layerBlocks2[b].Elements[e].Sy); // sy
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Type, layerBlocks2[b].Elements[e].Type); // type
                }
            }
        }

        [TestMethod]
        public void TestReader04()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 1000.0f, 9000.0f, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03),
                    }),
                new LayerBlock(1, 2, 1000.0f, 9000.0f, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable),
                    }),
            };

            var stream = new MemoryStream();
            var writer = new WriterV1();
            writer.Write(layerBlocks1, new BinaryWriter(stream, Encoding.UTF8));

            stream.Position = 0;

            var reader = new ReaderV1();
            var layerBlocks2 = reader.Read(new BinaryReader(stream, Encoding.UTF8)) as LayerBlock[];
            Assert.IsNotNull(layerBlocks2);

            Assert.AreEqual(layerBlocks1.Length, layerBlocks2.Length); // block count

            for (int b = 0; b < 2; b++)
            {
                Assert.AreEqual(layerBlocks1[b].Identifier, layerBlocks2[b].Identifier); // block id
                Assert.AreEqual(layerBlocks1[b].Difficulty, layerBlocks2[b].Difficulty); // block difficulty
                Assert.AreEqual(layerBlocks1[b].Width, layerBlocks2[b].Width); // block width
                Assert.AreEqual(layerBlocks1[b].Height, layerBlocks2[b].Height); // block height
                Assert.AreEqual(layerBlocks1[b].IsTakeOffBlock, layerBlocks2[b].IsTakeOffBlock); // block flags (take off)

                layerBlocks2[b].ReadData();
                Assert.IsNotNull(layerBlocks2[b].Elements);

                Assert.AreEqual(layerBlocks1[b].Elements.Length, layerBlocks2[b].Elements.Length); // block element count

                for (int e = 0; e < 3; e++)
                {
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Tx, layerBlocks2[b].Elements[e].Tx); // tx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Ty, layerBlocks2[b].Elements[e].Ty); // ty
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Angle, layerBlocks2[b].Elements[e].Angle); // angle
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sx, layerBlocks2[b].Elements[e].Sx); // sx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sy, layerBlocks2[b].Elements[e].Sy); // sy
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Type, layerBlocks2[b].Elements[e].Type); // type
                }
            }
        }
    }
}
