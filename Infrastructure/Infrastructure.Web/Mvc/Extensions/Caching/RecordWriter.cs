﻿// -----------------------------------------------------------------------
//  <copyright file="RecordWriter.cs" company="DLYB">
//      Copyright (c) 2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 13:16</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Infrastructure.Web.Mvc.Extensions
{
    internal class RecordWriter : TextWriter
    {
        private readonly TextWriter _innerWriter;
        private readonly List<StringBuilder> _recorders = new List<StringBuilder>();

        public RecordWriter(TextWriter innerWriter)
        {
            _innerWriter = innerWriter;
        }

        public override Encoding Encoding { get { return _innerWriter.Encoding; } }

        public override void Write(char value)
        {
            _innerWriter.Write(value);

            if (_recorders.Count > 0)
            {
                foreach (StringBuilder recorder in _recorders)
                {
                    recorder.Append(value);
                }
            }
        }

        public override void Write(string value)
        {
            if (value != null)
            {
                _innerWriter.Write(value);

                if (_recorders.Count > 0)
                {
                    foreach (StringBuilder recorder in _recorders)
                    {
                        recorder.Append(value);
                    }
                }
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _innerWriter.Write(buffer, index, count);

            if (_recorders.Count > 0)
            {
                foreach (StringBuilder recorder in _recorders)
                {
                    recorder.Append(buffer, index, count);
                }
            }
        }

        public void AddRecorder(StringBuilder recorder)
        {
            _recorders.Add(recorder);
        }

        public void RemoveRecorder(StringBuilder recorder)
        {
            _recorders.Remove(recorder);
        }
    }
}