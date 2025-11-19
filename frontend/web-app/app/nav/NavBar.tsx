import React from 'react'
import { AiOutlineCar } from 'react-icons/ai';

export default function NavBar() {
  return (
    <header className="sticky top-0 z-50 flex h-16 w-full items-center
     justify-between bg-white px-4 shadow-md text-gray-800">
          <div className="flex items-center gap-2 text-3xl font-semibold text-blue-500">
            <AiOutlineCar size={30}/>
            <div>Cars Auction</div>
        </div>
        <div>Search</div>
        <div>Login</div>
    </header>
  )
}
