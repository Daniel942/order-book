import { useEffect, useState } from 'react';

function AuditLog() {
    const [auditLog, setAuditLog] = useState([]);

    useEffect(() => {
        fetch('https://localhost:7117/api/getauditlog')
            .then(response => {
                if (response.ok) {
                    return response.json();
                }

                throw response;
            })
            .then(data => {
                if (data) {
                    // Make timestamps presentable
                    setAuditLog(data.map(item => ({...item, timestamp: new Date(item.timestamp * 1000).toLocaleString()}) ));
                }
            })
            .catch(error => {
                console.error(`Error fetching data: ${error}`);
            });
        }, []);

    return (
        <>
            {
                auditLog && auditLog.map(date => <a id={date.id} key={date.id} href={`/order-book/${date.id}`}>{ date.timestamp }</a>)
            }
        </>
    );
}

export default AuditLog;