import React, { useState, /*useEffect*/ } from 'react';
import axios from 'axios';
import display_image from './PicArray';
import { PutCartUpdate, TokenHeader } from '../ListOfLinks';

export const CartItem = (props) => {
    const [count, setCount] = useState(props.value.quantity);

    const [/*removeItem*/, setRemoveItem] = useState(false);

    const UpdateCart = async () => {
        await axios.put(PutCartUpdate(props.value.offering_key, count), {}, TokenHeader(props.token)).then((res) => {});
        window.location.href = '/cart';
    }
  
    const presentCounter = (<div type="number" className="h-10 font-semibold flex justify-center text-gray-500 w-1/3 items-center text-2xl" > {count} </div>) 
    const changeCounter = (<div type="number" className="h-10 font-semibold flex justify-center text-gray-800 w-1/3 items-center text-2xl" > {count} </div>)             
    const changeUpdate = (<button onClick={() => UpdateCart()} className="flex justify-center text-base rounded border-2 border-orange-500 px-6 mr-4 font-bold">Update Cart</button>)              
    const changeUpdateMobile = (<button onClick={() => UpdateCart()} className="justify-center mx-20 my-4 rounded hover:bg-orange-400 border-2 border-orange-500 px-5 font-bold">Update Cart</button>)

    const counters = (
        <div className="justify-center m-4 rounded h-11 border-2 border-orange-500">
            <div className="flex font-semibold hover:text-black focus:text-black text-gray-700" >
                <button onClick={() => minusItem()}className=" flex justify-center rounded text-gray-600 hover:text-gray-700 hover:bg-orange-400 h-full w-1/3 border-r-2 border-orange-500">
                    <div className="mx-20 flex items-center text-2xl h-10">-</div>
                </button>
                {count === props.value.quantity ? presentCounter : changeCounter }
                <button onClick={() => setCount(count + 1)} className="flex justify-center rounded text-gray-700 hover:text-gray-700 hover:bg-orange-400 h-full text-right w-1/3 border-l-2 border-orange-500">
                    <div className="flex items-center text-2xl h-10" >+</div>
                </button>
            </div>
        </div>
    );
    
    const minusItem = ()=>{
        setCount(count - 1);
        if (count === 1) { setCount(1) } 
    }
    
    const removeCartItem = async() => {
        await axios.put(PutCartUpdate(props.value.offering_key, 0), {}, TokenHeader(props.token)).then((res) => { setRemoveItem(true); });
        window.location.href = '/cart';
    }

    const linktoProduct = () => {
        window.location.assign("/offering/" + props.value.offering_key);
    }
    var multi2 = (a, b) => {
        return (a * b).toFixed(2);
    }
    const container = (<div>
        <div className="flex w-full">
            <div className="w-full">
                <div className="flex rounded border border-orange-300 w-full">
                    <div className="w-1/2 md:w-1/3">
                        <div onClick={() => linktoProduct()} className="flex m-2 h-32 md:h-56">{display_image()}</div>
                        <div className="md:hidden"> 
                        {counters}
                            {count === props.value.quantity ? <> 
                                <div className="font-bold text-center text-gray-500"> Total: ${props.value.totalOfferingCost} </div> </> :
                                <>
                                    <div className="font-bold text-center "> Total: ${multi2(props.value.unit_retail, count)}</div>
                                    <div>{changeUpdateMobile}</div>
                                </>
                            }   
                        </div>
                    </div>
                    
                    <div className="block text-lg w-2/3">
                        <div onClick={() => linktoProduct()} className="cursor-pointer m-4 text-md md:text-2xl font-extrabold hover:text-orange-500 underline">{props.value.product_name}</div>
                        <div className="m-4 text-md md:text-2xl font-extrabold pt-2">${props.value.unit_retail}</div>
                        <button onClick={() => removeCartItem()} className="flex justify-center text-base m-20 h-12 rounded border-2 border-orange-500 px-5 font-bold md:hidden">Remove Item</button>                    

                    

                    </div>
                    <div className="hidden md:block md:w-1/4 justify-center ">
                        <div className="m-0 md:mr-2">{counters}</div>
                        {count === props.value.quantity ? <> 
                            <div className="font-bold text-center text-gray-500 md:mr-4 md:pb-2"> Total: ${props.value.totalOfferingCost} </div> </> :
                            <>
                                <div className="font-bold text-center"> Total: ${multi2(props.value.unit_retail, count)}</div> 
                                <div className="my-2">{changeUpdate}</div>
                                    </>
                                }
                
                        <button onClick={() => removeCartItem()} className="flex justify-center text-base rounded border-2 border-orange-500 px-6 mr-12 md:mr-4 font-bold">Remove Item</button>
                        
                    </div>
                </div>
               
            </div>
           
        </div> 
     
    </div> )
    
    return (<>
        {container}  
    </>);
}

export const CartItemCheckOut = (props) => {
 
    const linktoProduct = () => {
        window.location.assign("/offering/" + props.value.offering_key);
    }
    const container = (<div>
        <div className="flex w-full ">
            <div className="flex rounded border border-orange-300 w-full">
                <div onClick={() => linktoProduct()} className="flex w-full h-48 md:w-1/4 md:h-56">{display_image()}</div>
                <div className="block text-lg w-full">
                    <div onClick={() => linktoProduct()} className="cursor-pointer m-4 text-md md:text-2xl font-extrabold  hover:text-orange-500 hover:underline">{props.value.product_name}</div>
                    <div className="m-4 text-2xl font-extrabold pt-2">${props.value.unit_retail}</div>
                </div>
              
                <div className="flex-none">
                    <div className="text-base md:text-xl font-bold text-right pt-4 pr-2">
                        Quantity: {props.value.quantity}
                    </div>
                    <div className="text-base md:text-xl font-bold text-right pt-4 pr-2">
                       Total Cost: ${props.value.totalOfferingCost}
                    </div>
                </div>
            </div>
        </div> 
    </div>)

    return (<>
        {container}
    </>);
}
