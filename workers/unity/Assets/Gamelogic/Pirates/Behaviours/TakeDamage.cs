
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours
{
	// Add this MonoBehaviour on UnityWorker (server-side) workers only
	[WorkerType(WorkerPlatform.UnityWorker)]
	public class TakeDamage : MonoBehaviour
	{
		// Enable this MonoBehaviour only on the worker with write access for the entity's Health component
		[Require] private Health.Writer HealthWriter;

		private void OnTriggerEnter(Collider other)
		{
			/*
             * Unity's OnTriggerEnter runs even if the MonoBehaviour is disabled, so non-authoritative UnityWorkers
             * must be protected against null writers
             */
			if (HealthWriter == null)
				return;

			// Ignore collision if this ship is already dead
			if (HealthWriter.Data.currentHealth <= 0)
				return;

			if (other != null && other.gameObject.tag == "Cannonball")
			{
				// Reduce health of this entity when hit
				int newHealth = HealthWriter.Data.currentHealth - 250;
				HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
			}
		}
	}
}