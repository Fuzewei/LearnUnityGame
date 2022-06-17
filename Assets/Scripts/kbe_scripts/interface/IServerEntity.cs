namespace KBEngine
{
	using GameLogic;
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;

	public interface IServerEntity
	{
		public GameEntity renderEntity { get; set; } //和渲染层交互的接口对象

		public TimeLineManager timeLineManager { get; set; } //和渲染层交互的接口对象

		public void onRenderObjectCreate(GameEntity render);

		public void uploadPositionAndRotation(params object[] args);

		public bool isBeControl();

	}
}