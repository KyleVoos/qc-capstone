import React, { useState, useEffect } from 'react';
import axios from 'axios';
import display_image from './PicArray';

export const CheckOut = () => {
    return (<>
    <div className="h-auto">
        <div className=" titlePage pt-2 pb-4 lg:text-3xl"> Check Out</div>
        <div className="flex w-full">
            <div className="text-lg pl-2 md:text-2xl w-1/2">Shipping Address</div>
            
            <div className="text-sm pr-2 underline w-1/2 text-right">Change</div>
        </div>
        <hr className="m-2 pb-1 px-4 bg-orange-500"/>
        <div className="text-lg pl-2 md:text-2xl w-1/2">Payment Method</div>
        <hr className="m-2 pb-1 px-4 bg-orange-500"/>
        <div className="w-full">
            <div className="text-base md:text-xl pt-4 pl-2">Name on the Card</div>
            <input id="cardname" className="m-2 rounded border border-gray-600 h-10 px-5 pr-8 text-sm w-70 md:w-1/3" type="search" name="search"></input>
            <div className="text-base md:text-xl pt-4 pl-2">Card Number</div>
            <input id="cardname" className="m-2 rounded border border-gray-600 h-10 px-5 pr-8 text-sm w-70 md:w-1/3" type="search" name="search"></input>
            <div className="text-base md:text-xl pt-4 px-2 block md:w-1/3 md:flex">Expirantion Date MM/YY</div>
            <input id="cardname" className="m-2 block md:flex rounded border border-gray-600 h-10 px-5 pr-8 text-sm w-70 md:w-1/3" type="search" name="search"></input>
            <div className="text-base md:text-xl pt-4 pl-2 block md:flex">CSV</div>
            <input id="cardname" className="m-2 block md:flex rounded border border-gray-600 h-10 px-5 pr-8 text-sm w-70 md:w-1/3" type="search" name="search"></input>
        </div>
        <div className="text-lg pl-2 pt-4 md:text-2xl w-1/2">Order Review</div>
        <hr className="m-2 pb-1 px-4 bg-orange-500"/>
        <div className="flex w-full /*bg-yellow-400*/">
                    <div className="flex border border-orange-300 w-full m-2">
                        <div className="flex w-full h-48 md:w-1/4 md:h-56">{display_image()}</div>
                        <div className="block text-lg w-full">
                            <div className="m-4 text-md md:text-2xl font-extrabold /*bg-purple-500*/">Heavy Ducimus Bucket</div>
                                <div className="m-4 text-2xl font-extrabold pt-2">$143.21</div>
                                <div className="pl-4 pt-2">ErrorIpsa CO.</div>
                                <div className="pl-4 pt-2">Quantity: 5</div>
                        </div>
                    </div>
                </div>
                <div className="flex w-full /*bg-yellow-400*/">
                    <div className="flex border border-orange-300 w-full m-2">
                        <div className="flex w-full h-48 md:w-1/4 md:h-56">{display_image()}</div>
                        <div className="block text-lg w-full">
                            <div className="m-4 text-md md:text-2xl font-extrabold /*bg-purple-500*/">Heavy Ducimus Bucket</div>
                                <div className="m-4 text-2xl font-extrabold pt-2">$143.21</div>
                                <div className="pl-4 pt-2">ErrorIpsa CO.</div>
                                <div className="pl-4 pt-2">Quantity: 5</div>
                        </div>
                    </div>
                </div>
                <div className="flex w-full /*bg-yellow-400*/">
                    <div className="flex border border-orange-300 w-full m-2">
                        <div className="flex w-full h-48 md:w-1/4 md:h-56">{display_image()}</div>
                        <div className="block text-lg w-full">
                            <div className="m-4 text-md md:text-2xl font-extrabold /*bg-purple-500*/">Heavy Ducimus Bucket</div>
                                <div className="m-4 text-2xl font-extrabold pt-2">$143.21</div>
                                <div className="pl-4 pt-2">ErrorIpsa CO.</div>
                                <div className="pl-4 pt-2">Quantity: 5</div>
                        </div>
                    </div>
                </div>
        </div> 
        <button className="rounded h-12 w-70 md:h-12 m-2 md:w-64 bg-white border-black border-2 text-base" >Place Order</button>
         
        </>)
}