﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using LayerDataReaderWriter.V5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LayerDataReaderWriter.UnitTests
{
    [TestClass]
    public class UnitTestV5
    {
        [TestMethod]
        public void TestWriter01()
        {
            var writer = new WriterV5();

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
            var writer = new WriterV5();
            Assert.AreEqual(writer.Version, 5u);
        }

        [TestMethod]
        public void TestWriter03()
        {
            var stream = new MemoryStream();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 10.0f, 90.0f, new[] { true, false, true, false, true, false, true, false }, new SystemFlags(true),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(0, 0.1f, 2.3f, 0.25f, 0.75f, 4.5f, 6.7f, 8.9f, 1, null),
                        new LayerBlockElement(1, 2.3f, 4.5f, 0.75f, 0.25f, 6.7f, 8.9f, 0.1f, 2,
                            new Component[]
                            {
                                new Component("A", new ComponentProperty(null, true)),
                                new Component("B", new ComponentProperty(null, false)),
                            }
                        ),
                        new LayerBlockElement(2, 4.5f, 6.7f, 0.5f, 1.0f, 8.9f, 0.1f, 2.3f, 3,
                            new Component[]
                            {
                                new Component("A", new ComponentProperty(null, 1)),
                                new Component("B", new ComponentProperty(null, -2)),
                                new Component("A", new ComponentProperty(null, 0.1f)),
                                new Component("A", new ComponentProperty(null, -0.2f)),
                            }
                        ),
                    }
                ),
                new LayerBlock(1, 2, 100.0f, 900.0f, new[] { false, true, false, true, false, true, false, true }, new SystemFlags(false),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(3, 0.1f, 2.3f, 0.0f, 0.5f, 4.5f, 6.7f, 8.9f, 4, null),
                        new LayerBlockElement(4, 2.3f, 4.5f, 0.1f, 0.9f, 6.7f, 8.9f, 0.1f, 5,
                            new Component[]
                            {
                                new Component("A1", new ComponentProperty(null, "test")),
                                new Component("B2", new ComponentProperty(null, guid1)),
                                new Component("C3", new ComponentProperty(null, guid2)),
                            }
                        ),
                        new LayerBlockElement(5, 4.5f, 6.7f, 0.9f, 0.1f, 8.9f, 0.1f, 2.3f, 6,
                            new Component[]
                            {
                                new Component("D4", new ComponentProperty(null, "this is a long text")),
                                new Component("E5", null),
                                new Component("F6", new ComponentProperty(null, "this is another long text")),
                            }
                        ),
                    }
                ),
            };

            var writer = new WriterV5();
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
            Assert.AreEqual(sr.ReadByte(), 0x55); // block user flags
            Assert.AreEqual(sr.ReadByte(), 1); // block system flags

            var blockSize = sr.ReadUInt32(); // block byte size
            var startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadUInt16(), 0); // ID
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.25f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.75f); // py
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual(sr.ReadByte(), 1); // type
            Assert.AreEqual(sr.ReadByte(), 0); // number of components

            Assert.AreEqual(sr.ReadUInt16(), 1); // ID
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.75f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.25f); // py
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual(sr.ReadByte(), 2); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 1); // component property type bool
            Assert.AreEqual(sr.ReadByte(), 1); // component property value true
            Assert.AreEqual(sr.ReadByte(), 1); // index of B in string table
            Assert.AreEqual(sr.ReadByte(), 1); // component property type bool
            Assert.AreEqual(sr.ReadByte(), 0); // component property value false

            Assert.AreEqual(sr.ReadUInt16(), 2); // ID
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.5f); // px
            Assert.AreEqual(sr.ReadSingle(), 1.0f); // py
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual(sr.ReadByte(), 3); // type
            Assert.AreEqual(sr.ReadByte(), 4); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 2); // component property type int
            Assert.AreEqual(sr.ReadInt32(), 1); // component property value 1
            Assert.AreEqual(sr.ReadByte(), 1); // index of B in string table
            Assert.AreEqual(sr.ReadByte(), 2); // component property type int
            Assert.AreEqual(sr.ReadInt32(), -2); // component property value -2
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 3); // component property type float
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // component property value 0.1
            Assert.AreEqual(sr.ReadByte(), 0); // index of A in string table
            Assert.AreEqual(sr.ReadByte(), 3); // component property type float
            Assert.AreEqual(sr.ReadSingle(), -0.2f); // component property value -0.2

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 100.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 900.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 0xAA); // block user flags
            Assert.AreEqual(sr.ReadByte(), 0); // black system flags

            blockSize = sr.ReadUInt32(); // block byte size
            startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadUInt16(), 3); // ID
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.0f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.5f); // py
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual(sr.ReadByte(), 4); // type
            Assert.AreEqual(sr.ReadByte(), 0); // number of components

            Assert.AreEqual(sr.ReadUInt16(), 4); // ID
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.9f); // py
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual(sr.ReadByte(), 5); // type
            Assert.AreEqual(sr.ReadByte(), 3); // number of components
            Assert.AreEqual(sr.ReadByte(), 2); // index of A1 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 4); // component property string length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(4)), "test"); // component property value "test"
            Assert.AreEqual(sr.ReadByte(), 3); // index of B2 in string table
            Assert.AreEqual(sr.ReadByte(), 5); // component property type Guid
            Assert.AreEqual(new Guid(sr.ReadBytes(16)), guid1); // component property value equal to guid1 variable
            Assert.AreEqual(sr.ReadByte(), 4); // index of C3 in string table
            Assert.AreEqual(sr.ReadByte(), 5); // component property type Guid
            Assert.AreEqual(new Guid(sr.ReadBytes(16)), guid2); // component property value equal to guid1 variable

            Assert.AreEqual(sr.ReadUInt16(), 5); // ID
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.9f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // py
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual(sr.ReadByte(), 6); // type
            Assert.AreEqual(sr.ReadByte(), 3); // number of components
            Assert.AreEqual(sr.ReadByte(), 5); // index of D4 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 19); // component property string length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(19)), "this is a long text"); // component property value "this is a long text"
            Assert.AreEqual(sr.ReadByte(), 6); // index of E5 in string table
            Assert.AreEqual(sr.ReadByte(), 0); // component property type none
            Assert.AreEqual(sr.ReadByte(), 7); // index of F6 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 25); // component property string length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(25)), "this is another long text"); // component property value "this is another long text"

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestWriter04()
        {
            var guid = Guid.NewGuid();

            var layerBlocks = new LayerBlock[]
            {
                new LayerBlock(0, 1, 20.0f, 180.0f, new[] { false, true, true, false, true, false, false, true }, new SystemFlags(false),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(10, 0.1f, 2.3f, 0.8f, 0.2f, 4.5f, 6.7f, 8.9f, 3,
                            new Component[]
                            {
                                new Component("Type1", new ComponentProperty(null, false)),
                                new Component("Type2", null),
                            }
                        ),
                        new LayerBlockElement(20, 2.3f, 4.5f, 0.1f, 0.2f, 6.7f, 8.9f, 0.1f, 4,
                            new Component[]
                            {
                                new Component("Type3", new ComponentProperty(null, -37)),
                                new Component("Type4", new ComponentProperty(null, 51)),
                            }
                        ),
                        new LayerBlockElement(30, 4.5f, 6.7f, 0.3f, 0.7f, 8.9f, 0.1f, 2.3f, 5,
                            new Component[]
                            {
                                new Component("Type5", new ComponentProperty(null, 123.456f)),
                                new Component("Type6", new ComponentProperty(null, -987.654f)),
                            }
                        ),
                    }
                ),
                new LayerBlock(1, 2, 200.0f, 1800.0f, new[] { true, false, false, true, false, true, true, false }, new SystemFlags(true),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(40, 0.1f, 2.3f, 0.25f, 0.75f, 4.5f, 6.7f, 8.9f, 6,
                            new Component[]
                            {
                                new Component("Type7", new ComponentProperty(null, "Type7")),
                                new Component("Type8", new ComponentProperty(null, "Type8")),
                            }
                        ),
                        new LayerBlockElement(50, 2.3f, 4.5f, 0.75f, 0.25f, 6.7f, 8.9f, 0.1f, 7,
                            new Component[]
                            {
                                new Component("Type9", new ComponentProperty(null, null)),
                                new Component("Type10", new ComponentProperty(null, "")),
                            }
                        ),
                        new LayerBlockElement(60, 4.5f, 6.7f, 0.5f, 0.5f, 8.9f, 0.1f, 2.3f, 8,
                            new Component[]
                            {
                                new Component("Type11", new ComponentProperty(null, Guid.Empty)),
                                new Component("Type12", new ComponentProperty(null, guid)),
                            }
                        ),
                    }
                ),
            };

            uint version = 5;

            var stream = new MemoryStream();
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
            Assert.AreEqual(sr.ReadByte(), 0x96); // block user flags
            Assert.AreEqual(sr.ReadByte(), 0); // block system flags

            var blockSize = sr.ReadUInt32(); // block byte size
            var startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadUInt16(), 10); // ID
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.8f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.2f); // py
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual(sr.ReadByte(), 3); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 0); // index of Type1 in string table
            Assert.AreEqual(sr.ReadByte(), 1); // component property type bool
            Assert.AreEqual(sr.ReadByte(), 0); // component property value false
            Assert.AreEqual(sr.ReadByte(), 1); // index of Type2 in string table
            Assert.AreEqual(sr.ReadByte(), 0); // component property type none

            Assert.AreEqual(sr.ReadUInt16(), 20); // ID
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.2f); // py
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual(sr.ReadByte(), 4); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 2); // index of Type3 in string table
            Assert.AreEqual(sr.ReadByte(), 2); // component property type int
            Assert.AreEqual(sr.ReadInt32(), -37); // component property value -37
            Assert.AreEqual(sr.ReadByte(), 3); // index of Type4 in string table
            Assert.AreEqual(sr.ReadByte(), 2); // component property type int
            Assert.AreEqual(sr.ReadInt32(), 51); // component property value 51

            Assert.AreEqual(sr.ReadUInt16(), 30); // ID
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.3f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.7f); // py
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual(sr.ReadByte(), 5); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 4); // index of Type5 in string table
            Assert.AreEqual(sr.ReadByte(), 3); // component property type float
            Assert.AreEqual(sr.ReadSingle(), 123.456f); // component property value 123.456
            Assert.AreEqual(sr.ReadByte(), 5); // index of Type6 in string table
            Assert.AreEqual(sr.ReadByte(), 3); // component property type float
            Assert.AreEqual(sr.ReadSingle(), -987.654f); // component property value -987.654

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.ReadByte(), 1); // block id
            Assert.AreEqual(sr.ReadByte(), 2); // block difficulty
            Assert.AreEqual(sr.ReadSingle(), 200.0f); // block width
            Assert.AreEqual(sr.ReadSingle(), 1800.0f); // block height
            Assert.AreEqual(sr.ReadByte(), 0x69); // block user flags
            Assert.AreEqual(sr.ReadByte(), 1); // block system flags

            blockSize = sr.ReadUInt32(); // block byte size
            startPos = sr.BaseStream.Position;

            Assert.AreEqual(sr.ReadUInt16(), 3); // block element count

            Assert.AreEqual(sr.ReadUInt16(), 40); // ID
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // tx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.25f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.75f); // py
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // angle
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // sx
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sy
            Assert.AreEqual(sr.ReadByte(), 6); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 6); // index of Type7 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 5); // component property string value length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(5)), "Type7"); // component property value "Type7"
            Assert.AreEqual(sr.ReadByte(), 7); // index of Type8 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 5); // component property string value length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(5)), "Type8"); // component property value "Type8"

            Assert.AreEqual(sr.ReadUInt16(), 50); // ID
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // tx
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.75f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.25f); // py
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // angle
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // sx
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sy
            Assert.AreEqual(sr.ReadByte(), 7); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 8); // index of Type9 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 0); // component property string value length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(0)), ""); // component property value ""
            Assert.AreEqual(sr.ReadByte(), 9); // index of Type10 in string table
            Assert.AreEqual(sr.ReadByte(), 4); // component property type string
            Assert.AreEqual(sr.ReadUInt16(), 0); // component property string value length
            Assert.AreEqual(Encoding.UTF8.GetString(sr.ReadBytes(0)), ""); // component property value ""

            Assert.AreEqual(sr.ReadUInt16(), 60); // ID
            Assert.AreEqual(sr.ReadSingle(), 4.5f); // tx
            Assert.AreEqual(sr.ReadSingle(), 6.7f); // ty
            Assert.AreEqual(sr.ReadSingle(), 0.5f); // px
            Assert.AreEqual(sr.ReadSingle(), 0.5f); // py
            Assert.AreEqual(sr.ReadSingle(), 8.9f); // angle
            Assert.AreEqual(sr.ReadSingle(), 0.1f); // sx
            Assert.AreEqual(sr.ReadSingle(), 2.3f); // sy
            Assert.AreEqual(sr.ReadByte(), 8); // type
            Assert.AreEqual(sr.ReadByte(), 2); // number of components
            Assert.AreEqual(sr.ReadByte(), 10); // index of Type11 in string table
            Assert.AreEqual(sr.ReadByte(), 5); // component property type Guid
            Assert.AreEqual(new Guid(sr.ReadBytes(16)), Guid.Empty); // component property value Guid.Empty
            Assert.AreEqual(sr.ReadByte(), 11); // index of Type12 in string table
            Assert.AreEqual(sr.ReadByte(), 5); // component property type Guid
            Assert.AreEqual(new Guid(sr.ReadBytes(16)), guid); // component property value equal to guid variable

            Assert.AreEqual(blockSize, sr.BaseStream.Position - startPos);

            Assert.AreEqual(sr.BaseStream.Position, sr.BaseStream.Length);
        }

        [TestMethod]
        public void TestReader01()
        {
            var reader = new ReaderV5();

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
            var reader = new ReaderV5();
            Assert.AreEqual(reader.Version, 5u);
        }

        [TestMethod]
        public void TestReader03()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 30.0f, 270.0f, Enumerable.Range(0, 8).Select(i => NextBool()).ToArray(), new SystemFlags(NextBool()),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(100, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 1,
                            new Component[]
                            {
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                                new Component(NextString(), null),
                                new Component(NextString(), new ComponentProperty(null, NextGuid())),
                                new Component(NextString(), new ComponentProperty(null, NextString())),
                                new Component(NextString(), new ComponentProperty(null, NextInt())),
                            }
                        ),
                        new LayerBlockElement(200, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 2,
                            new Component[]
                            {
                                new Component(NextString(), new ComponentProperty(null, NextBool())),
                                new Component(NextString(), new ComponentProperty(null, NextString())),
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                                new Component(NextString(), null),
                                new Component(NextString(), new ComponentProperty(null, NextGuid())),
                            }
                        ),
                        new LayerBlockElement(300, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 3,
                            new Component[]
                            {
                                new Component(NextString(), new ComponentProperty(null, NextGuid())),
                                new Component(NextString(), new ComponentProperty(null, null)),
                                new Component(NextString(), new ComponentProperty(null, NextInt())),
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                                new Component(NextString(), new ComponentProperty(null, NextBool())),
                            }
                        ),
                    }
                ),
                new LayerBlock(1, 2, 300.0f, 2700.0f, Enumerable.Range(0, 8).Select(i => NextBool()).ToArray(), new SystemFlags(NextBool()),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(400, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 4,
                            new Component[]
                            {
                                new Component(NextString(), null),
                                new Component(NextString(), new ComponentProperty(null, NextBool())),
                                new Component(NextString(), new ComponentProperty(null, NextGuid())),
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                                new Component(NextString(), new ComponentProperty(null, NextString())),
                            }
                        ),
                        new LayerBlockElement(500, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 5,
                            new Component[]
                            {
                                new Component(NextString(), new ComponentProperty(null, NextBool())),
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                                new Component(NextString(), new ComponentProperty(null, NextString())),
                                new Component(NextString(), new ComponentProperty(null, NextInt())),
                                new Component(NextString(), null),
                            }
                        ),
                        new LayerBlockElement(600, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 6,
                            new Component[]
                            {
                                new Component(NextString(), new ComponentProperty(null, NextGuid())),
                                new Component(NextString(), new ComponentProperty(null, NextBool())),
                                new Component(NextString(), null),
                                new Component(NextString(), new ComponentProperty(null, NextInt())),
                                new Component(NextString(), new ComponentProperty(null, NextFloat())),
                            }
                        ),
                    }
                ),
            };

            var stream = new MemoryStream();

            ReaderWriterManager.Write(layerBlocks1, stream, Encoding.UTF8, 5);

            stream.Position = 0;

            var layerBlocks2 = ReaderWriterManager.Read(stream, Encoding.UTF8) as LayerBlock[];
            Assert.IsNotNull(layerBlocks2);

            Assert.AreEqual(layerBlocks1.Length, layerBlocks2.Length); // block count

            for (int b = 0; b < layerBlocks1.Length; b++)
            {
                Assert.AreEqual(layerBlocks1[b].Identifier, layerBlocks2[b].Identifier); // block id
                Assert.AreEqual(layerBlocks1[b].Difficulty, layerBlocks2[b].Difficulty); // block difficulty
                Assert.AreEqual(layerBlocks1[b].Width, layerBlocks2[b].Width); // block width
                Assert.AreEqual(layerBlocks1[b].Height, layerBlocks2[b].Height); // block height
                for (int f = 0; f < 8; f++)
                    Assert.AreEqual(layerBlocks1[b].UserFlags[f], layerBlocks2[b].UserFlags[f]); // block user flag
                Assert.AreEqual(layerBlocks1[b].SystemFlags.IsEnabled, layerBlocks2[b].SystemFlags.IsEnabled); // block system flag

                Assert.IsNotNull(layerBlocks2[b].Elements);

                Assert.AreEqual(layerBlocks1[b].Elements.Length, layerBlocks2[b].Elements.Length); // block element count

                for (int e = 0; e < layerBlocks1[b].Elements.Length; e++)
                {
                    Assert.AreEqual(layerBlocks1[b].Elements[e].ID, layerBlocks2[b].Elements[e].ID); // ID
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Tx, layerBlocks2[b].Elements[e].Tx); // tx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Ty, layerBlocks2[b].Elements[e].Ty); // ty
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Angle, layerBlocks2[b].Elements[e].Angle); // angle
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sx, layerBlocks2[b].Elements[e].Sx); // sx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sy, layerBlocks2[b].Elements[e].Sy); // sy
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Type, layerBlocks2[b].Elements[e].Type); // type

                    Assert.AreEqual(layerBlocks1[b].Elements[e].Components.Length, layerBlocks2[b].Elements[e].Components.Length); // component element count

                    for (int c = 0; c < layerBlocks1[b].Elements[e].Components.Length; c++)
                    {
                        Assert.AreEqual(layerBlocks1[b].Elements[e].Components[c].Type, layerBlocks2[b].Elements[e].Components[c].Type);

                        var prop1 = layerBlocks1[b].Elements[e].Components[c].Property;
                        var prop2 = layerBlocks2[b].Elements[e].Components[c].Property;

                        Assert.AreEqual(prop1 == null, prop2 == null);
                        if (prop1 != null)
                        {
                            Assert.AreEqual(prop1.Name, prop2.Name);
                            Assert.AreEqual(prop1.Type, prop2.Type);
                            Assert.AreEqual(prop1.BooleanValue, prop2.BooleanValue);
                            Assert.AreEqual(prop1.IntegerValue, prop2.IntegerValue);
                            Assert.AreEqual(prop1.FloatValue, prop2.FloatValue);
                            Assert.AreEqual(prop1.StringValue, prop2.StringValue);
                            Assert.AreEqual(prop1.GuidValue, prop2.GuidValue);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestReader04()
        {
            var layerBlocks1 = new LayerBlock[]
            {
                new LayerBlock(0, 1, 1000.0f, 9000.0f, Enumerable.Range(0, 8).Select(i => NextBool()).ToArray(), new SystemFlags(NextBool()),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(1000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 1, new Component[0]),
                        new LayerBlockElement(2000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 2,
                            new Component[]
                            {
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                            }
                        ),
                        new LayerBlockElement(3000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 3,
                            new Component[]
                            {
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                           }
                        ),
                    }
                ),
                new LayerBlock(1, 2, 1000.0f, 9000.0f, Enumerable.Range(0, 8).Select(i => NextBool()).ToArray(), new SystemFlags(NextBool()),
                    new LayerBlockElement[]
                    {
                        new LayerBlockElement(4000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 4,
                            new Component[]
                            {
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                            }
                        ),
                        new LayerBlockElement(5000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 5, new Component[0]),
                        new LayerBlockElement(6000, NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), NextFloat(), 6,
                            new Component[]
                            {
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                                new Component(NextString(), NextComponentProperty()),
                            }
                        ),
                    }
                ),
            };

            var stream = new MemoryStream();

            var writer = new WriterV5();
            writer.Write(layerBlocks1, new BinaryWriter(stream, Encoding.UTF8));

            stream.Position = 0;

            var reader = new ReaderV5();

            var layerBlocks2 = reader.Read(new BinaryReader(stream, Encoding.UTF8)) as LayerBlock[];
            Assert.IsNotNull(layerBlocks2);

            Assert.AreEqual(layerBlocks1.Length, layerBlocks2.Length); // block count

            for (int b = 0; b < layerBlocks1.Length; b++)
            {
                Assert.AreEqual(layerBlocks1[b].Identifier, layerBlocks2[b].Identifier); // block id
                Assert.AreEqual(layerBlocks1[b].Difficulty, layerBlocks2[b].Difficulty); // block difficulty
                Assert.AreEqual(layerBlocks1[b].Width, layerBlocks2[b].Width); // block width
                Assert.AreEqual(layerBlocks1[b].Height, layerBlocks2[b].Height); // block height
                for (int f = 0; f < 8; f++)
                    Assert.AreEqual(layerBlocks1[b].UserFlags[f], layerBlocks2[b].UserFlags[f]); // block user flag
                Assert.AreEqual(layerBlocks1[b].SystemFlags.IsEnabled, layerBlocks2[b].SystemFlags.IsEnabled); // block system flag

                Assert.IsNotNull(layerBlocks2[b].Elements);

                Assert.AreEqual(layerBlocks1[b].Elements.Length, layerBlocks2[b].Elements.Length); // block element count

                for (int e = 0; e < layerBlocks1[b].Elements.Length; e++)
                {
                    Assert.AreEqual(layerBlocks1[b].Elements[e].ID, layerBlocks2[b].Elements[e].ID); // tx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Tx, layerBlocks2[b].Elements[e].Tx); // tx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Ty, layerBlocks2[b].Elements[e].Ty); // ty
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Angle, layerBlocks2[b].Elements[e].Angle); // angle
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sx, layerBlocks2[b].Elements[e].Sx); // sx
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Sy, layerBlocks2[b].Elements[e].Sy); // sy
                    Assert.AreEqual(layerBlocks1[b].Elements[e].Type, layerBlocks2[b].Elements[e].Type); // type

                    Assert.AreEqual(layerBlocks1[b].Elements[e].Components.Length, layerBlocks2[b].Elements[e].Components.Length);

                    var n = layerBlocks1[b].Elements[e].Components.Length;
                    for (var c = 0; c < n; c++)
                    {
                        Assert.AreEqual(layerBlocks1[b].Elements[e].Components[c].Type, layerBlocks2[b].Elements[e].Components[c].Type);

                        var prop1 = layerBlocks1[b].Elements[e].Components[c].Property;
                        var prop2 = layerBlocks2[b].Elements[e].Components[c].Property;

                        Assert.AreEqual(prop1 == null, prop2 == null);
                        if (prop1 != null)
                        {
                            Assert.AreEqual(prop1.Name, prop2.Name);
                            Assert.AreEqual(prop1.Type, prop2.Type);
                            Assert.AreEqual(prop1.BooleanValue, prop2.BooleanValue);
                            Assert.AreEqual(prop1.IntegerValue, prop2.IntegerValue);
                            Assert.AreEqual(prop1.FloatValue, prop2.FloatValue);
                            Assert.AreEqual(prop1.StringValue, prop2.StringValue);
                            Assert.AreEqual(prop1.GuidValue, prop2.GuidValue);
                        }
                    }
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

        private Random rnd = new Random(Guid.NewGuid().GetHashCode());

        private bool NextBool()
        {
            return rnd.Next(2) == 1;
        }

        private int NextInt()
        {
            return rnd.Next();
        }

        private float NextFloat()
        {
            return (float)(rnd.NextDouble() * 999.999);
        }

        private string NextString()
        {
            var len = rnd.Next(10, 20);
            var bytes = Enumerable
                .Range(0, len)
                .Select(_ => (byte)(rnd.Next(2) == 0 ? rnd.Next(97, 123) : rnd.Next(65, 91)))
                .ToArray();
            return Encoding.UTF8.GetString(bytes);
        }

        private Guid NextGuid()
        {
            var buffer = new byte[16];
            rnd.NextBytes(buffer);
            return new Guid(buffer);
        }

        private ComponentProperty NextComponentProperty()
        {
            switch (rnd.Next(6))
            {
                case 0: return new ComponentProperty(null, NextBool());
                case 1: return new ComponentProperty(null, NextInt());
                case 2: return new ComponentProperty(null, NextFloat());
                case 3: return new ComponentProperty(null, NextString());
                case 4: return new ComponentProperty(null, NextGuid());
            }
            return null; // this case can happen
        }
    }
}
