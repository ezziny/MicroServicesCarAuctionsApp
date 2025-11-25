'use client'

import React from 'react'
import Countdown, { zeroPad } from 'react-countdown';

const renderer = ({ days, hours, minutes, seconds, completed }:{days : number, hours: number, minutes: number, seconds: number, completed: boolean} ) => {
  const bgColor = completed
    ? "bg-red-500"
    : (days < 1 && hours < 10)
      ? "bg-orange-300"
      : "bg-green-500";

  return (
    <div
      className={`border-3 border-white text-white py-1 px-2 rounded-lg flex justify-center ${bgColor}`}
     suppressHydrationWarning={true}>
      {completed
        ? "Auction Ended"
        : `${days}:${zeroPad(hours)}:${zeroPad(minutes)}:${zeroPad(seconds)}`}
    </div>
  );
};
type Props = { auctionEnd: string }
export default function Timer({auctionEnd}: Props) {
  return (
    <div>
        <Countdown date={auctionEnd} renderer={renderer}/>
    </div>
  )
}
