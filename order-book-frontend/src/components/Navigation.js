import { NavLink } from 'react-router-dom';
import '../styles/navigation.scss';

function Navigation() {
    return (
        <>
            <nav>
                <NavLink to='/order-book' className={({ isActive }) => isActive ? 'active' : ''}>Order Book</NavLink>
                <NavLink to='/audit-log' className={({ isActive }) => isActive ? 'active' : ''}>Audit Log</NavLink>
            </nav>
        </>
    );
}

export default Navigation;