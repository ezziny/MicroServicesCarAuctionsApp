import Image from 'next/image'
import React from 'react'
import Timer from './Timer'

type Props = {
    auction: any
}

export default function AuctionCard({auction}: Props) {
  return (
    <a href="#" >
          <div className="w-full bg-gray-200 aspect-video rounded-lg relative overflow-hidden">
            <Image
              src={auction.imageUrl}
              alt='Car Auction Image'
              fill
              className="object-cover"
              priority
              sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"

              />
              <div className="absolute bottom-2 left-2">
                 <Timer auctionEnd={auction.auctionEnd} />
              </div>
          </div>
            <div className="flex justify-between items-center mt-4">
                <h3 className="text-gray-900"> {auction.make} {auction.model}</h3>
                <p className="font-semibold text-sm"> {auction.year}</p>
            </div>
    </a>
  )
}
