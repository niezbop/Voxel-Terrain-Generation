using UnityEngine;

namespace Generation {
	public interface IVOXGenerator {
		VOXObject GenerateVOX(Gradient colors);
	}
}
