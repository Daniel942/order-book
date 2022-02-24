import { useEffect, useState, useCallback } from 'react';
import '../styles/trade.scss';

function Trade() {
    const [units, setUnits] = useState(0);
    const [asks, setAsks] = useState([]);
    const [message, setMessage] = useState('');

    function handleChange(event) {
        const value = event.target.value;
        setUnits(value);

        updatePrice();
    }

    const updatePrice = useCallback(() => {
        if (units && !isNaN(units - 0) && units > 0) {
            // Can not do anything if data is not present
            if (!asks) {
                return;
            }

            // Calculate the price
            let gotPrice = false;
            let currentPrice = 0;
            let currentValue = 0;
            for (let i = 0; i < asks.length; i++) {
                const price = parseFloat(asks[i][0]);
                const amount = parseFloat(asks[i][1]);

                // Using the whole ask pair does not cover all the units requested
                if (currentValue + amount < units) {
                    currentPrice += price * amount;
                    currentValue += amount;
                }
                // Found enough units to calculate the full price
                else {
                    const residualAmount = units - currentValue;
                    currentPrice += price * residualAmount;
                    currentValue += residualAmount;
                    gotPrice = true;
                    break;
                }
            }

            // Calculated the price
            if (gotPrice) {
                setMessage(`Price is ${currentPrice.toFixed(2)} EUR for ${currentValue} BTC.`);
            }
            // Too few ask data present to calculate the requested price
            else {
                setMessage(`Can not retrieve price data for ${units} BTC.`);
            }
        }
        // 
        else {
            setMessage('');
        }
    }, [asks, units]);
    
    
    function handleSubmit(event) {
        console.log(`Submitting units: ${units}`);
        event.preventDefault();
    }

    useEffect(() => {
        // const ws = new WebSocket('wss://localhost:7117/ws');

        // Subscribe to Bitstamp order book WebSocket API
        const subscribeStructure = {
            'event': 'bts:subscribe',
            'data': {
                'channel': 'order_book_btceur'
            }
        };

        const ws = new WebSocket('wss://ws.bitstamp.net');

        ws.onopen = () => {
            ws.send(JSON.stringify(subscribeStructure));
        }

        ws.onmessage = (event) => {
            const response = JSON.parse(event.data);
            setAsks(response.data.asks);
            updatePrice();
        }

        ws.onclose = () => {
            ws.close();
        }
        
        return () => {
            ws.close();
        }
    }, [updatePrice]);

    return (
        <div className="card">
            <div className="card-title">
                <span>Buy BTC</span>
            </div>

            <div className="card-content">
                <div className="input">
                    <label>Units:</label>
                    <input type="text" value={units} onChange={handleChange} />
                </div>
                { message && <div className="message">{message}</div> }
            </div>
        </div>
    );
}

export default Trade;