import { useEffect, useState } from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import '../styles/order-book.scss';
import { useParams } from 'react-router-dom';

function OrderBook() {
    const [retrievedAt, setRetrievedAt] = useState(null);
    const [chartOptions, setChartOptions] = useState(null);

    const { id } = useParams("id");

    function onDataReceived(data) {
        // Make timestamp presentable
        setRetrievedAt(new Date(data.timestamp * 1000).toLocaleString());

        let totalBidAmount = 0;
        const bids = data.bids.map(bid => {
            // Highcharts library needs float values to work properly.
            const price = parseFloat(bid.price);
            const amount = parseFloat(bid.amount);
            totalBidAmount = totalBidAmount + amount;
            return [ price, totalBidAmount ];
        });

        let totalAskAmount = 0;
        const asks = data.asks.map(ask => {
            // Highcharts library needs float values to work properly.
            const price = parseFloat(ask.price);
            const amount = parseFloat(ask.amount);
            totalAskAmount = totalAskAmount + amount;
            return [ price, totalAskAmount ];
        });

        setChartOptions({
            chart: {
                type: 'area',
                zoomType: 'xy'
            },
            title: {
                text: 'Order book'
            },
            legend: {
                enabled: false
            },
            tooltip: {
                formatter: function () {
                    return `Total ${this.y} BTC at ${this.x} EUR`
                },
                valueDecimals: 8
            },
            plotOptions: {
                area: {
                    fillOpacity: 0.3,
                    lineWidth: 1,
                    step: 'center'
                }
            },
            series: [
                {
                    name: 'Bids',
                    data: bids.reverse(), // Reverse bids so they are ascending
                    color: '#1F8787',
                    xAxis: 0,
                },
                {
                    name: 'Asks',
                    data: asks,
                    color: '#DB3E3D',
                    xAxis: 1
                }
            ],
            xAxis: [
                {
                    title: {
                        text: 'Bid price'
                    },
                    width: '50%'
                },
                {
                    title: {
                        text: 'Ask price'
                    },
                    offset: 0,
                    width: '50%',
                    left: '50%'
                }
            ],
            yAxis: {
                title: {
                    text: 'Total amount'
                }
            }
        });
    }

    useEffect(() => {
        if (id) {
            fetch(`https://localhost:7117/api/getorderbookbyid?id=${id}`)
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    }

                    throw response;
                })
                .then(data => {
                    onDataReceived(data);
                })
                .catch(error => {
                    console.error(error);
                });
        }
        else {
            const ws = new WebSocket('wss://localhost:7117/ws/getorderbook');
    
            ws.onopen = () => {
                ws.send('btceur');
            }
    
            ws.onmessage = (event) => {
                const response = JSON.parse(event.data);
                onDataReceived(response);
                ws.send('btceur');
            }
    
            ws.onclose = () => {
                ws.close();
            }
            
            return () => {
                ws.close();
            }
        }
    }, [id]);

    return (
        <>
            { retrievedAt && chartOptions && <p>Retrieved at: {retrievedAt}</p>}

            {
                chartOptions && <HighchartsReact
                    highcharts={Highcharts}
                    options={chartOptions}
                />
            }
        </>
    );
}

export default OrderBook;