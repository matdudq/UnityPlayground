using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Procedural
{
	public partial class TerrainGenerator
	{
		private struct CreateMeshTrianglesJob : IJobParallelFor
		{
			[ReadOnly]
			public int meshResolution;
			
			[WriteOnly]
			public NativeArray<int> triangles;
			
			public void Execute(int index)
			{
				int indexModulo = index % 6;

				int triangleValue = -1;
				
				switch (indexModulo)
				{
					case 0:		
						triangleValue = index;
					break;
					case 1:
						triangleValue = index + meshResolution + 1;
					break;
					case 2:
						triangleValue = index + meshResolution;
					break;
					case 3:
						triangleValue = index + meshResolution + 1;
					break;
					case 4:
						triangleValue = index;
					break;
					case 5:
						triangleValue = index + 1;
					break;
				}

				triangles[index] = triangleValue;
			}
		}
	}
}