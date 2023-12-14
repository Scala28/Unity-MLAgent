# Unity-MLAgent
Little project to test the Unity MLAgent package and to gain experience in machine learning environment.
Managed to make this work with python 3.10.
Create the ML environment:
- navigate to the MLagents_test1 folder: cd <path to MLagents_test1 folder>
- create the python virtual environment: python -m venv venv
- activate the Virtual environment: venv\Scripts\activate
- upgrade pip library: python.exe -m pip install --upgrade pip
- installing the pyTorch packages: pip3 install torch==2.0.0+cu118 -f https://download.pytorch.org/whl/torch_stable.html
- Make sure of the correct numpy package version: pip3 uninstall numpy - and - pip3 install numpy==1.23.5
- Make sure of the correct protobuff package version: pip uninstall protobuf - and - pip install protobuf==3.20.3
- Install the onnx package: pip3 install onnx==1.13.1
- Navigate to the ml-agents_relase_20 folder: cd <path to ml-agents_relase_20 folder>
- Change the ml-agents-envs/setup.py configuration file: "numpy>=1.14.1,<1.24", (line 54) and delete line 60 
("numpy==1.21.2",)
- Install the ml-agents python packages: pip3 install -e ./ml-agents-envs - and - pip3 install -e ./ml-agents

Starting mlagents training:
- activate the Virtual Environment to run python dependencies: venv\Scripts\activate
- navigate to the project folder: cd <path to project folder>
- Start the ML-agents training: mlagents-learn config/CarAgent.yaml --initialize-from=*run_id* --run-id=*run_id* --resume/force

Check the training stats:
- activate the Virtual Environment: venv\Scripts\activate
- navigate to the project folder: cd <path to project folder>
- tensorboard --logdir results
