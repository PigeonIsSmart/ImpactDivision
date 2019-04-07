﻿using Unity.Entities;
using System;
using UnityEngine;
using UiEvent;

public class S_AttackListener : ComponentSystem {
	struct Group{
		public C_AttackListener _AttackListener;
        public C_Velocity _Velocity;
        public C_Attributes _Attributes;
        public C_Animator _Animator;
        public AudioSource _Audio;
    }

	protected override void OnUpdate()
	{
		foreach (var e in GetEntities<Group>())
		{
            var _attackListener = e._AttackListener;
            var _attackList = e._AttackListener.attackList;
            var _attribute = e._Attributes;
            var _velocity = e._Velocity;
            if (e._AttackListener.isActive)
            {
                foreach (Attack attack in _attackList)
                {
                    
                    _attackListener.photonView.RPC("Demaged", PhotonTargets.All, attack.demage);
                    
                    var source = attack.source;
                    var sourceAudio = source.GetComponent<AudioSource>();
                    var sourceIsLocalPlayer = source.GetComponent<C_Velocity>().isLocalPlayer;
                    var sourceBattleMgr = source.GetComponent<C_BattleMgr>();

                    sourceBattleMgr.AddDemage(attack.demage);
                    var hitMsg = new UiEvent.UiMsgs.Hit()
                    {
                        HeadShot = attack.headShot
                    };
                    source.GetComponent<C_UiEventMgr>().SendEvent(hitMsg);
                    Sound.PlayOneShot(sourceAudio, _attackListener.hitFeedBackSounds);

                    if (_attribute.HP <= 0)
                    {
                        sourceBattleMgr.AddKill();
                        Sound.PlayOneShot(sourceAudio, _attackListener.killSound);
                    }
                    
                }
                _attackList.Clear();
            }
        }
    }
}
