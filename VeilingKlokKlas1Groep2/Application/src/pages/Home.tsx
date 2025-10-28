import React from 'react';
import { useNavigate } from 'react-router-dom';

function Home() {
	const navigate = useNavigate();

	return (
		<div className="App">
			<nav className="App-nav">
				<button
					onClick={() => navigate('/Login')}
				>
					Login
				</button>
				<button
					onClick={() => navigate('/KlantDashboard')}
				>
					Dashboard
				</button>
			</nav>

			<header className="App-header">
				<img src={"/svg/logo.svg"} className="App-logo" alt="logo" />
				<p>
					Edit <code>src/Home.tsx</code> and save to reload.
				</p>
				<a className="App-link" href="https://reactjs.org" target="_blank" rel="noopener noreferrer">
					Learn React
				</a>
			</header>
		</div>
	);
}

export default Home;
