using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrapplingHook
{
	class SE_Grappled : StatusEffect
	{
		[Header("SE_Grappled")]
		public float m_minForce = 20f;

		public float m_maxForce = 30f;

		public float m_minDistance = 5f;

		public float m_maxDistance = 30f;

		public float m_staminaDrain = 0f;

		public float m_staminaDrainInterval = 0.1f;

		public float m_maxMass = 50f;

		private bool m_broken;

		private Character m_attacker;

		private float m_drainStaminaTimer;

		public override void Setup(Character character)
		{
			base.Setup(character);
		}

		public override void SetAttacker(Character attacker)
		{
			ZLog.Log("Setting attacker " + attacker.m_name);
			m_attacker = attacker;
			m_time = 0f;
			if (Vector3.Distance(m_attacker.transform.position, m_character.transform.position) > m_maxDistance)
			{
				m_attacker.Message(MessageHud.MessageType.Center, "Target too far");
				m_broken = true;
				return;
			}
			m_attacker.Message(MessageHud.MessageType.Center, m_character.m_name + " Grappled");
			GameObject[] startEffectInstances = m_startEffectInstances;
			foreach (GameObject gameObject in startEffectInstances)
			{
				if ((bool)gameObject)
				{
					LineConnect component = gameObject.GetComponent<LineConnect>();
					if ((bool)component)
					{
						component.SetPeer(m_attacker.GetComponent<ZNetView>());
					}
				}
			}
		}

		public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
			if (!m_attacker)
			{
				return;
			}
			Rigidbody component = m_character.GetComponent<Rigidbody>();
			if (!component)
			{
				return;
			}
			Vector3 vector = m_character.transform.position - m_attacker.transform.position;
			Vector3 normalized = vector.normalized;
			float radius = m_character.GetRadius();
			float magnitude = vector.magnitude;
			float num = Mathf.Clamp01(Vector3.Dot(normalized, component.velocity));
			float t = Utils.LerpStep(m_minDistance, m_maxDistance, magnitude);
			float num2 = Mathf.Lerp(m_minForce, m_maxForce, t);
			float num3 = Mathf.Clamp01(m_maxMass / component.mass);
			float num4 = num2 * num3;
			if (magnitude - radius > m_minDistance && num < num4)
			{
				//if (Math.Abs(m_character.transform.position.y - m_attacker.transform.position.y) < 0.1)
    //            {
				//	normalized.y = 0f;
				//}
				normalized.Normalize();
				if (m_character.GetStandingOnShip() == null && !m_character.IsAttached())
				{
					m_attacker.GetComponent<Rigidbody>().AddForce(normalized * num4, ForceMode.VelocityChange);
				}
				m_drainStaminaTimer += dt;
				if (m_drainStaminaTimer > m_staminaDrainInterval)
				{
					m_drainStaminaTimer = 0f;
					float num5 = 1f - Mathf.Clamp01(num / num2);
					m_attacker.UseStamina(m_staminaDrain * num5);
				}
			}
			if (magnitude > m_maxDistance)
			{
				m_broken = true;
				m_attacker.Message(MessageHud.MessageType.Center, "Line broke");
			}
			if (!m_attacker.HaveStamina())
			{
				m_broken = true;
				m_attacker.Message(MessageHud.MessageType.Center, m_character.m_name + " escaped");
			}
		}

		public override bool IsDone()
		{
			if (base.IsDone())
			{
				return true;
			}
			if (m_broken)
			{
				return true;
			}
			if (!m_attacker)
			{
				return true;
			}
			if (m_time > 2f && (m_attacker.IsBlocking() || m_attacker.InAttack()))
			{
				m_attacker.Message(MessageHud.MessageType.Center, m_character.m_name + " released");
				return true;
			}
			if ((m_character.transform.position - m_attacker.transform.position).magnitude < m_minDistance)
			{
				var mBody = AccessTools.Field(typeof(Character), "m_body").GetValue(m_attacker) as Rigidbody;
				mBody.velocity = Vector3.zero;
				return true;
			}
			return false;
		}
	}

}
