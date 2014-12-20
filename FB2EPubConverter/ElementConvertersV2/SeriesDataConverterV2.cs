﻿using System.Linq;
using EPubLibrary;
using FB2Library;

namespace FB2EPubConverter.ElementConvertersV2
{
    internal class SeriesDataConverterV2
    {
        public void Convert(FB2File fb2File, EPubFileV2 epubFile)
        {
            epubFile.Collections.CollectionMembers.Clear();
            foreach (var seq in fb2File.TitleInfo.Sequences)
            {
                if (!string.IsNullOrEmpty(seq.Name))
                {
                    var collectionMember = new CollectionMember
                    {
                        CollectionName = seq.Name,
                        Type = CollectionType.Series,
                        CollectionPosition = seq.Number
                    };
                    epubFile.Collections.CollectionMembers.Add(collectionMember);
                    foreach (var subseq in seq.SubSections.Where(subseq => !string.IsNullOrEmpty(subseq.Name)))
                    {
                        collectionMember = new CollectionMember
                        {
                            CollectionName = subseq.Name,
                            Type = CollectionType.Set,
                            CollectionPosition = subseq.Number
                        };
                        epubFile.Collections.CollectionMembers.Add(collectionMember);
                    }
                }
            }           
        }
    }
}
