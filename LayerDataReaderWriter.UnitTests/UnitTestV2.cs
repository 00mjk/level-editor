using System;
using System.IO;
using System.Text;
using LayerDataReaderWriter.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LayerDataReaderWriter.UnitTests
{
    [TestClass]
    public class UnitTestV2
    {
        [TestMethod]
        public void TestWriter01()
        {
            var writer = new WriterV2();

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
            var writer = new WriterV2();
            Assert.AreEqual(writer.Version, 2u);
        }

        [TestMethod]
        public void TestWriter03()
        {
            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 10.0f, 90.0f, true, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01, null),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02,
                            new Component[]
                            {
                                new Component("A"),
                                new Component("B"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03,
                            new Component[]
                            {
                                new Component("A"),
                                new Component("B"),
                                new Component("A"),
                                new Component("A"),
                            }),
                    }),
                new LayerBlock(1, 2, 100.0f, 900.0f, false, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue, null),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple,
                            new Component[]
                            {
                                new Component("A1"),
                                new Component("B2"),
                                new Component("C3"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable,
                            new Component[]
                            {
                                new Component("D4"),
                                new Component("E5"),
                                new Component("F6"),
                            }),
                    }),
            };

            var stream = new MemoryStream();

            var writer = new WriterV2();
            writer.Write(layerBlocks, new BinaryWriter(stream, Encoding.UTF8));

            stream.Position = 0;

            var sr = new BinaryReader(stream);

            Assert.AreEqual(sr.ReadByte(), 8); // string items in the string table

            Assert.AreEqual(sr.ReadString(), "A");
            Assert.AreEqual(sr.ReadString(), "B");
            Assert.AreEqual(sr.ReadString(), "A1");
            Assert.AreEqual(sr.ReadString(), "B2");
            Assert.AreEqual(sr.ReadString(), "C3");
            Assert.AreEqual(sr.ReadString(), "D4");
            Assert.AreEqual(sr.ReadString(), "E5");
            Assert.AreEqual(sr.ReadString(), "F6");

            Assert.AreEqual(sr.ReadByte(), 2); // block count

            Assert.AreEqual(sr.ReadByte(), 0); // block id
            Assert.AreEqual(sr.ReadByte(), 1); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 10.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 90.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 1); // block flags (take off)

            var blockSize = sr.ReadUInt32(); // block byte size
            var startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv01); // type
            Assert.AreEqual(sr.ReadByte(), 0); // number of components

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv02); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 1); // index of B in string table

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv03); // type
            Assert.AreEqual(sr.ReadByte(), 4); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 1); // index of B in string table
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 100.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 900.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 2); // block flags (take off)

            blockSize = sr.ReadUInt32(); // block byte size
            startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemBlue); // type
            Assert.AreEqual(sr.ReadByte(), 0); // number of components

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemPurple); // type
            Assert.AreEqual(sr.ReadByte(), 3); // number of components
            Assert.AreEqual(sr.ReadByte(), 2); // index of A1 in string table
            Assert.AreEqual(sr.ReadByte(), 3); // index of B2 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // index of C3 in string table

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // txy
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleBreakable); // type
            Assert.AreEqual(sr.ReadByte(), 3); // number of components
            Assert.AreEqual(sr.ReadByte(), 5); // index of D4 in string table
            Assert.AreEqual(sr.ReadByte(), 6); // index of E5 in string table
            Assert.AreEqual(sr.ReadByte(), 7); // index of F6 in string table

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestWriter04()
        {
            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 20.0f, 180.0f, false, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv03,
                            new Component[]
                            {
                                new Component("Type1"),
                                new Component("Type2"),
                            }),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple,
                            new Component[]
                            {
                                new Component("Type3"),
                                new Component("Type4"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.GemRed,
                            new Component[]
                            {
                                new Component("Type5"),
                                new Component("Type6"),
                            }),
                    }),
                new LayerBlock(1, 2, 200.0f, 1800.0f, true, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.ObstacleBreakable,
                            new Component[]
                            {
                                new Component("Type7"),
                                new Component("Type8"),
                            }),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.ObstacleStatic,
                            new Component[]
                            {
                                new Component("Type9"),
                                new Component("Type10"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv01,
                            new Component[]
                            {
                                new Component("Type11"),
                                new Component("Type12"),
                            }),
                    }),
            };

            var stream = new MemoryStream();

            uint version = 2;
            ReaderWriterManager.Write(layerBlocks, stream, Encoding.UTF8, version);

            stream.Position = 0;

            var sr = new BinaryReader(stream);

            Assert.AreEqual(sr.ReadUInt32(), ReaderWriterManager.MagicNumber);
            Assert.AreEqual(sr.ReadUInt32(), version);

            Assert.AreEqual(sr.ReadByte(), 12); // string items in the string table

            Assert.AreEqual(sr.ReadString(), "Type1");
            Assert.AreEqual(sr.ReadString(), "Type2");
            Assert.AreEqual(sr.ReadString(), "Type3");
            Assert.AreEqual(sr.ReadString(), "Type4");
            Assert.AreEqual(sr.ReadString(), "Type5");
            Assert.AreEqual(sr.ReadString(), "Type6");
            Assert.AreEqual(sr.ReadString(), "Type7");
            Assert.AreEqual(sr.ReadString(), "Type8");
            Assert.AreEqual(sr.ReadString(), "Type9");
            Assert.AreEqual(sr.ReadString(), "Type10");
            Assert.AreEqual(sr.ReadString(), "Type11");
            Assert.AreEqual(sr.ReadString(), "Type12");

            Assert.AreEqual(sr.ReadByte(), 2); // block count

            Assert.AreEqual(sr.ReadByte(), 0); // block id
            Assert.AreEqual(sr.ReadByte(), 1); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 20.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 180.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 2); // block flags (take off)

            var blockSize = sr.ReadUInt32(); // block byte size
            var startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv03); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of Type1 in string table
            Assert.AreEqual(sr.ReadByte(), 1); // index of Type2 in string table

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemPurple); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 2); // index of Type3 in string table
            Assert.AreEqual(sr.ReadByte(), 3); // index of Type4 in string table

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.GemRed); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 4); // index of Type5 in string table
            Assert.AreEqual(sr.ReadByte(), 5); // index of Type6 in string table

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 200.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 1800.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 1); // block flags (take off)

            blockSize = sr.ReadUInt32(); // block byte size
            startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleBreakable); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 6); // index of Type7 in string table
            Assert.AreEqual(sr.ReadByte(), 7); // index of Type8 in string table

            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.ObstacleStatic); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 8); // index of Type9 in string table
            Assert.AreEqual(sr.ReadByte(), 9); // index of Type10 in string table

            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual((LayerBlockElementType)sr.ReadByte(), LayerBlockElementType.BoostLv01); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 10); // index of Type11 in string table
            Assert.AreEqual(sr.ReadByte(), 11); // index of Type12 in string table

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestReader01()
        {
            var reader = new ReaderV2();

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
            var reader = new ReaderV2();
            Assert.AreEqual(reader.Version, 2u);
        }

        [TestMethod]
        public void TestReader03()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 30.0f, 270.0f, false, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01,
                            new Component[]
                            {
                                new Component("filjsdfkA"),
                                new Component("tPfrfgfmdg"),
                                new Component("sdldfsdfdfkdf"),
                                new Component("iusfjDfkjvbdfhjv"),
                                new Component("eriryeUiopmcxvnxj"),
                            }),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02,
                            new Component[]
                            {
                                new Component("rfkJreflkjflk"),
                                new Component("oqwelKfssdf"),
                                new Component("weierkfjsElfkf"),
                                new Component("oqiwskdMadnasbn"),
                                new Component("rtuIertdjsnmg"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03,
                            new Component[]
                            {
                                new Component("rgiDfgldfmnmwe"),
                                new Component("weoieWjrasnmdnf"),
                                new Component("opdklsdmfMvzxc"),
                                new Component("ow3eurqwpoSdcsdcsc"),
                                new Component("erttproytrYhnmngbdftoi"),
                            }),
                    }),
                    new LayerBlock(1, 2, 300.0f, 2700.0f, true, false,
                        new LayerBlockElement[]
                        {
                            new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue,
                                new Component[]
                                {
                                    new Component("kdfjGldfgjkdflgkj"),
                                    new Component("yuojyUmyuonipjmy"),
                                    new Component("weytqwebvXf"),
                                    new Component("truiynvxcvNmdjkfhsd"),
                                    new Component("zgafsdgHasd"),
                                }),
                            new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple,
                                new Component[]
                                {
                                    new Component("rtTrigyghgh"),
                                    new Component("oirnYcbrurh"),
                                    new Component("ueRyugdfvhbfxdvmndfb"),
                                    new Component("cxmVndfyuighderui"),
                                    new Component("tyhitrusdmfnvXdgJH"),
                                }),
                            new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable,
                                new Component[]
                                {
                                    new Component("geruitherjfkvxSdn"),
                                    new Component("zxncmgeyrufwPe"),
                                    new Component("pwoeufshdfgvFrg"),
                                    new Component("ghoijertuiqLoper"),
                                    new Component("qwsascsayzfDgdfg"),
                                }),
                        }),
            };

            var stream = new MemoryStream();

            ReaderWriterManager.Write(layerBlocks1, stream, Encoding.UTF8, 2);

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
                Assert.AreEqual(layerBlocks1[b].IsEnabled, layerBlocks2[b].IsEnabled); // block flags (enabled)

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

                    Assert.AreEqual(layerBlocks1[b].Elements[e].Components.Length, layerBlocks2[b].Elements[e].Components.Length); // component element count

                    for (int c = 0; c < 5; c++)
                        Assert.AreEqual(layerBlocks1[b].Elements[e].Components[c].Type, layerBlocks2[b].Elements[e].Components[c].Type);
                }
            }
        }

        [TestMethod]
        public void TestReader04()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 1000.0f, 9000.0f, true, false,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.BoostLv01, new Component[0]),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.BoostLv02,
                            new Component[]
                            {
                                new Component("A"),
                                new Component("B"),
                                new Component("C"),
                                new Component("D"),
                                new Component("E"),
                                new Component("F"),
                                new Component("G"),
                                new Component("H"),
                                new Component("I"),
                                new Component("J"),
                                new Component("K"),
                                new Component("L"),
                                new Component("M"),
                            }),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.BoostLv03,
                            new Component[]
                            {
                                new Component("N"),
                                new Component("O"),
                                new Component("P"),
                                new Component("Q"),
                                new Component("R"),
                                new Component("S"),
                                new Component("T"),
                                new Component("U"),
                                new Component("V"),
                                new Component("W"),
                                new Component("X"),
                                new Component("Y"),
                                new Component("Z"),
                            }),
                    }),
                new LayerBlock(1, 2, 1000.0f, 9000.0f, false, true,
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0.1f, 2.3f, 4.5f, 6.7f, 8.9f, LayerBlockElementType.GemBlue,
                            new Component[]
                            {
                                new Component("TypeEnMousse37"),
                                new Component("TypeEnMousse28"),
                                new Component("TypeEnMousse53"),
                                new Component("TypeEnMousse12"),
                                new Component("TypeEnMousse88"),
                                new Component("TypeEnMousse43"),
                            }),
                        new LayerBlockElement(2.3f, 4.5f, 6.7f, 8.9f, 0.1f, LayerBlockElementType.GemPurple, new Component[0]),
                        new LayerBlockElement(4.5f, 6.7f, 8.9f, 0.1f, 2.3f, LayerBlockElementType.ObstacleBreakable,
                            new Component[]
                            {
                                new Component("TypeEnMousse28"),
                                new Component("TypeEnMousse62"),
                                new Component("TypeEnMousse34"),
                                new Component("TypeEnMousse89"),
                                new Component("TypeEnMousse36"),
                                new Component("TypeEnMousse12"),
                            }),
                    }),
            };

            var stream = new MemoryStream();

            var writer = new WriterV2();
            writer.Write(layerBlocks1, new BinaryWriter(stream, Encoding.UTF8));

            stream.Position = 0;

            var reader = new ReaderV2();
            var layerBlocks2 = reader.Read(new BinaryReader(stream, Encoding.UTF8)) as LayerBlock[];

            Assert.AreEqual(layerBlocks1.Length, layerBlocks2.Length); // block count

            for (int b = 0; b < 2; b++)
            {
                Assert.AreEqual(layerBlocks1[b].Identifier, layerBlocks2[b].Identifier); // block id
                Assert.AreEqual(layerBlocks1[b].Difficulty, layerBlocks2[b].Difficulty); // block difficulty
                Assert.AreEqual(layerBlocks1[b].Width, layerBlocks2[b].Width); // block width
                Assert.AreEqual(layerBlocks1[b].Height, layerBlocks2[b].Height); // block height
                Assert.AreEqual(layerBlocks1[b].IsTakeOffBlock, layerBlocks2[b].IsTakeOffBlock); // block flags (take off)
                Assert.AreEqual(layerBlocks1[b].IsEnabled, layerBlocks2[b].IsEnabled); // block flags (enabled)

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

                    Assert.AreEqual(layerBlocks1[b].Elements[e].Components.Length, layerBlocks2[b].Elements[e].Components.Length);

                    var n = layerBlocks1[b].Elements[e].Components.Length;
                    for (var c = 0; c < n; c++)
                        Assert.AreEqual(layerBlocks1[b].Elements[e].Components[c].Type, layerBlocks2[b].Elements[e].Components[c].Type);
                }
            }
        }

        [TestMethod]
        public void TestStringTable01()
        {
            var stringTable = new StringTable(1);

            Assert.AreEqual(stringTable.Add("A"), 0);
            Assert.AreEqual(stringTable.Add("A"), 0);
            Assert.AreEqual(stringTable.Add("A"), 0);
            Assert.AreNotEqual(stringTable.Add("B"), 0);

            Assert.AreEqual(stringTable.Count, 2u);

            try
            {
                stringTable.IndexOf("C");
                throw new InvalidOperationException();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void TestStringTable02()
        {
            var st = new StringTable(1);

            for (int i = 0; i < 256; i++)
                st.Add(Guid.NewGuid().ToString("N"));

            Assert.AreEqual(st.Count, 256u);

            try
            {
                st.Add(Guid.NewGuid().ToString("N"));
                throw new InvalidOperationException();
            }
            catch
            {
            }

            Assert.AreEqual(st.Count, 256u);
        }
    }
}
