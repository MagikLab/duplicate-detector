﻿using DuplicateDetectorUWP.Detector.Enumerable;
using DuplicateDirectorUWP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DuplicateDetectorUWP.Detector
{
    public class GroupRecord
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public long Size { get; internal set; }
        public string TypeGroup { get; internal set; }
        public ObservableCollection<Record> records { get; set; }

        // Constructor
        public GroupRecord()
        {
            this.Id = Singleton.CreateGuid();
            this.Name = string.Empty;
            this.Comment = string.Empty;
            this.records = new ObservableCollection<Record>();
            this.Size = 0;
            this.TypeGroup = string.Empty;
        }
        
        public void AddRecord(Record record)
        {
            if (record == null)
            {
                throw new Exception("Can't add null object!");
            }
            this.records.Add(record);
        }

        public void RemoveRecord(int index)
        {
            if(index < 0)
            {
                throw new Exception("Index invalid!");
            }
            this.records.RemoveAt(index);
        }
        
        public void ClearRecords()
        {
            if(this.records == null)
            {
                throw new Exception("List of records was null!");
            }
            this.records.Clear();
        }

        //Priority: OldestFile/NewestFile -> SmallestSize/LargestSize -> LongestName/ShortestName 
        internal void DetectOriginRecord(EnumerableDetectOrigin[] detectorOrigin)
        {
            if (detectorOrigin == null)
                throw new Exception("Param can't null");
            if((detectorOrigin.Contains(EnumerableDetectOrigin.NewestFile) && detectorOrigin.Contains(EnumerableDetectOrigin.OldestFile))
                || (detectorOrigin.Contains(EnumerableDetectOrigin.LargestSize) && detectorOrigin.Contains(EnumerableDetectOrigin.SmallestSize))
                || (detectorOrigin.Contains(EnumerableDetectOrigin.LongestName) && detectorOrigin.Contains(EnumerableDetectOrigin.ShortestName)))
            {
                throw new Exception("Can't choose both NewestFile and OldestFile or both LargestSize and SmallestSize or both LongestName and ShortestName of EnumerableDetectOrigin");
            }
            
            List<Record> gr = records.ToList<Record>();

            //
            if (detectorOrigin.Contains(EnumerableDetectOrigin.LongestName))
            {
                gr = gr.OrderByDescending(item =>
                {
                    item.IsOrigin = false;
                    return item.Name.Length;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.Name.Length == gr[0].Name.Length);
            }
            else if (detectorOrigin.Contains(EnumerableDetectOrigin.ShortestName))
            {
                gr = gr.OrderBy(item =>
                {
                    item.IsOrigin = false;
                    return item.Name.Length;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.Name.Length == gr[0].Name.Length);
            }

            //
            if (detectorOrigin.Contains(EnumerableDetectOrigin.LargestSize))
            {
                gr = gr.OrderByDescending(item =>
                {
                    item.IsOrigin = false;
                    return item.Size;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.Size == gr[0].Size);
            }
            else if (detectorOrigin.Contains(EnumerableDetectOrigin.SmallestSize))
            {
                gr = gr.OrderBy(item =>
                {
                    item.IsOrigin = false;
                    return item.Size;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.Size == gr[0].Size);
            }

            //
            if (detectorOrigin.Contains(EnumerableDetectOrigin.NewestFile))
            {
                gr = gr.OrderByDescending(item =>
                {
                    item.IsOrigin = false;
                    return item.DateCreated;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.DateCreated == gr[0].DateCreated);
            }
            else if (detectorOrigin.Contains(EnumerableDetectOrigin.OldestFile))
            {
                gr = gr.OrderBy(item =>
                {
                    item.IsOrigin = false;
                    return item.DateCreated;
                }).ToList<Record>();
                //gr = gr.FindAll(item => item.DateCreated == gr[0].DateCreated);
            }

            gr[0].IsOrigin = true;
            for (int i = 1; i < gr.Count; i++)
            {
                if (((detectorOrigin.Contains(EnumerableDetectOrigin.NewestFile) || detectorOrigin.Contains(EnumerableDetectOrigin.OldestFile))
                            && gr[0].DateCreated.Equals(gr[i].DateCreated)) 
                    && ((detectorOrigin.Contains(EnumerableDetectOrigin.SmallestSize) || detectorOrigin.Contains(EnumerableDetectOrigin.LargestSize)) 
                            && gr[0].Size.Equals(gr[i].Size))
                    && ((detectorOrigin.Contains(EnumerableDetectOrigin.LongestName) || detectorOrigin.Contains(EnumerableDetectOrigin.ShortestName)) 
                            && gr[0].Name.Equals(gr[i].Name)))
                {
                    gr[i].IsOrigin = true;
                }
                else
                {
                    break;
                }
            }
        }
    }
}