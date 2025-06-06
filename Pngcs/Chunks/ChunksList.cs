using System;
using System.Collections.Generic;
using System.Text;

namespace Pngcs.Chunks
{
    /// <summary>
    /// All chunks that form an image, read or to be written.<br/>
    /// http://www.w3.org/TR/PNG/#table53
    /// </summary>
    public class ChunksList
    {
        internal const int CHUNK_GROUP_0_IDHR = 0; // required - single
        internal const int CHUNK_GROUP_1_AFTERIDHR = 1; // optional - multiple
        internal const int CHUNK_GROUP_2_PLTE = 2; // optional - single
        internal const int CHUNK_GROUP_3_AFTERPLTE = 3; // optional - multple
        internal const int CHUNK_GROUP_4_IDAT = 4; // required (single pseudo chunk)
        internal const int CHUNK_GROUP_5_AFTERIDAT = 5; // optional - multple
        internal const int CHUNK_GROUP_6_END = 6; // only 1 chunk - requried

        /// <summary>
        ///  Includes all chunks, but IDAT is a single pseudo chunk without data
        /// </summary>
        protected List<PngChunk> _chunks;

        /// <summary>
        /// Creates a new <see cref="ChunksList"/> from the given <see cref="ImageInfo"/>.
        /// </summary>
        /// <param name="imageInfo">The image info.</param>
        internal ChunksList()
        {
            _chunks = [];
        }

        /// <summary>
        /// Keys of processed (read or written) chunks.
        /// </summary>
        /// <returns>key:chunk id, val: number of occurrences</returns>
        public Dictionary<string, int> GetChunksKeys()
        {
            var ck = new Dictionary<string, int>();
            foreach (PngChunk c in _chunks)
            {
                ck[c.Id] = ck.TryGetValue(c.Id, out int value) ? value + 1 : 1;
            }
            return ck;
        }

        /// <summary>
        /// Returns a copy of the chunk list (but the chunks are not copied) .
        /// </summary>
        /// <remarks>
        /// This should not be used for general metadata handling.
        /// </remarks>
        /// <returns>A new list of the same chunks.</returns>
        public List<PngChunk> GetChunks()
            => new(_chunks);

        internal static List<PngChunk> GetXById(List<PngChunk> list, string id, string? innerid)
        {
            if (innerid == null)
                return ChunkHelper.FilterList(list, new ChunkPredicateId(id));

            return ChunkHelper.FilterList(list, new ChunkPredicateId2(id, innerid));
        }

        /// <summary>
        /// Adds chunk in next position. This is used only by the pngReader
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkGroup"></param>
        public void AppendReadChunk(PngChunk chunk, int chunkGroup)
        {
            chunk.ChunkGroup = chunkGroup;
            _chunks.Add(chunk);
        }

        /// <summary>
        /// All chunks with this ID
        /// </summary>
        /// <remarks>The GetBy... methods never include queued chunks</remarks>
        /// <param name="id"></param>
        /// <returns>List, empty if none</returns>
        public List<PngChunk> GetListById(string id)
            => GetListById(id, null);

        /// <summary>
        /// Same as ID, but we an additional discriminator for textual keys
        /// </summary>
        /// <remarks>If innerid!=null and the chunk is PngChunkTextVar or PngChunkSPLT, it's filtered by that id</remarks>
        /// <param name="id"></param>
        /// <param name="innerid">Only used for text and SPLT chunks</param>
        /// <returns>List, empty if none</returns>
        public List<PngChunk> GetListById(string id, string? innerid)
            => GetXById(_chunks, id, innerid);

        /// <summary>
        /// Returns only one chunk 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>First chunk found, null if not found</returns>
        public PngChunk? GetById(string id)
            => GetById(id, false);

        /// <summary>
        /// Returns only one chunk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="failIfMultiple">true, and more than one found: exception</param>
        /// <returns>null if not found</returns>
        public PngChunk? GetById(string id, bool failIfMultiple)
            => GetById1(id, null, failIfMultiple);

        /// <summary>
        /// Sames as <c>GetById1(String id, bool failIfMultiple)</c> but allows an additional innerid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="innerid"></param>
        /// <param name="failIfMultiple">true, and more than one found: exception</param>
        /// <returns>null if not found</returns>
        public PngChunk? GetById1(string id, string? innerid, bool failIfMultiple)
        {
            List<PngChunk> list = GetListById(id, innerid);
            if (list.Count == 0)
                return null;
            if (list.Count > 1 && (failIfMultiple || !list[0].AllowsMultiple()))
                throw new Exception($"Unexpected duplicate chunk IDs: {id}");

            return list[^1];
        }

        /// <summary>
        /// Finds all chunks "equivalent" to this one
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns>Empty if nothing found</returns>
        public List<PngChunk> GetEquivalent(PngChunk chunk)
            => ChunkHelper.FilterList(_chunks, new ChunkPredicateEquiv(chunk));

        /// <summary>
        /// Only the amount of chunks
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Chunks: {_chunks.Count}";

        /// <summary>
        /// Detailed information for debugging.
        /// </summary>
        /// <returns>Debugging information.</returns>
        public string ToStringFull()
        {
            var sb = new StringBuilder(ToString());
            sb.Append("\n Read:\n");
            foreach (PngChunk chunk in _chunks)
            {
                sb.Append(chunk).Append($" Group: {chunk.ChunkGroup}\n");
            }
            return sb.ToString();
        }
    }
}
