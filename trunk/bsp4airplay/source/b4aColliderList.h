#pragma once
#include <IwManagedList.h>
#include <Ib4aCollider.h>
namespace Bsp4Airplay
{
	class Cb4aColliderList
	{
		CIwArray<Ib4aCollider*> m_List;

		public:
		//Constructor
		Cb4aColliderList();
		//Desctructor
		virtual ~Cb4aColliderList();
		inline void push_back(Ib4aCollider* c){m_List.push_back(c);}
		inline uint32 GetSize() const {return (uint32)m_List.size();}
		inline void SerialiseHeader() {m_List.SerialiseHeader();}
		inline const Ib4aCollider*operator[](uint32 i) const {return m_List[i];}
		void	Delete();
		void	SerialisePtrs();
		void	Serialise();
	};
}