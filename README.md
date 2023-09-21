# Racing Simulator : 자동차 시뮬레이터 (2022.08.02)


## **[23.09.21]**
### [Review]
1. 1년 2개월 만에 보는 프로젝트
2. 기존 게임 구조를 유지하는 선에서 유지보수가 더 낫도록 깔끔한 코드로 수정하거나, 메모리 사용량을 줄이는 방향으로 수정해보겠다.
3. UI 생성 및 레이어 구조는 별도로 건드리지 않고, 씬 자체에 사전 존재하는 기존 형태 그대로 두겠다.
4. Addressable 등의 리소스 로드 방식은 사용하지 않고 현행 유지하겠다.
5. 인코딩에 대해 아무것도 몰랐던 때라 한글 주석이 다 깨져있다. UTF-8 로 다 바꾸어야겠다.

### [Refactoring]



---

## 1. 개요

---

- 최초 제작 기간 : 22.07.28 ~ 22.08.06
- 마지막 수정 : 23.09.21

---

유니티에서 휠 콜라이더를 사용한 자동차의 움직임을 구현하고 시뮬레이션의 성향을 띈 주행 게임

을 구현하고자 했다. 아케이드 성을 배제하고, 자동차의 물리를 중심으로 시뮬레이터를 제작했다.

자동차는 3종류가 선택이 가능하며, 실제 차와 동일한 스펙을 가지도록 설계하였다. 차종마다 구동

방식에 맞춰 FF, FR, RR 의 구동방식을 구현하였다. 키보드 입력도 가능하나, 전용 휠 컨트롤러

(게임패드)사용에 초점을 맞추었다. 테스트한 기기는 Logitech G29 이며, 나머지 기기는 테스트가

불가능하여 지원 목록에 추가하지 않았다.

## 2. 구현

---

### 1. 바퀴의 구현

---
![image](https://user-images.githubusercontent.com/65384983/206628653-cc7a5ee0-663b-4252-9de6-33abdb6b934e.png)


자동차의 바퀴는 유니티에서 제공하는 **Wheel Collider** 를 사용하였다. 자동차 모델의 크기에 맞게

**Wheel Collider** 의 반지름을 설정하고 적절한 위치에 배치하였다.

**Wheel Collider** 4개를 코드 상에서 가져오는 방법은 다음과 같다.

```csharp
protected class WheelInfo // Left and Right Wheels
    {
        public WheelCollider Left_Wheel;
        public WheelCollider Right_Wheel;
    }
```

좌측, 우측의 바퀴를 **WheelInfo** 라는 하나의 객체로 만들어 사용한다.

**WheelInfo** 는 **List<WheelInfo>** 를 통해 앞바퀴와 뒷바퀴로 분리하여 사용한다.

```csharp
protected List<WheelInfo> Wheels = new List<WheelInfo>(); // Wheels List

WheelInfo Front = new WheelInfo();
WheelInfo Back = new WheelInfo();

Wheels.Add(Front);
Wheels.Add(Back);
```

**Wheels[0]**는 앞바퀴에 대한 정보를, **Wheels[1]**은 뒷바퀴에 대한 정보를 담고 있다.

```csharp
// Steer
Wheels[0].Left_Wheel.steerAngle = Steering;
Wheels[0].Right_Wheel.steerAngle = Steering;
// Motor
Wheels[0].Left_Wheel.motorTorque = Motor;
Wheels[0].Right_Wheel.motorTorque = Motor;
// Brake
Wheels[0].Left_Wheel.brakeTorque = Brake;
Wheels[0].Right_Wheel.brakeTorque = Brake;
```

바퀴의 움직임은 **Wheel Collider**의 **motorTorque, brakeTorque, steerAngle** 속성을 사용하여

움직임을 구현하였다.

### 2. 비주얼 휠 (Visual Wheel)

---
![image](https://user-images.githubusercontent.com/65384983/206628790-cabd665a-b43d-410e-9df5-405525dae671.png)

각 바퀴는 **Wheel Collider** 와 별개로 눈에 보이는 **비주얼 휠(Visual Wheel)**이 존재한다.

```csharp
protected virtual void MoveVisualWheel(WheelCollider wheel)
    {
        wheel.GetWorldPose(out colliderWorldPos, out colliderWorldRot);
        visualWheel = wheel.transform.GetChild(0).gameObject;
        visualWheel.transform.position = colliderWorldPos;
        visualWheel.transform.rotation = colliderWorldRot;
    }
```

비주얼 휠은 **Wheel Collider**의 **WorldPosition, WorldRotation** 값을 가져와 자신에게 적용하여,

**Wheel Collider**가 움직이는 만큼 같은 움직임을 구사한다.

### 3. 차량의 구동 방식과 무게 중심

---

- **FF (Front engine Front wheel drive) 방식 : Audi A3**

---

![image](https://user-images.githubusercontent.com/65384983/206628847-90f84dee-00d9-4087-b5ca-66651ee0f16e.png)

아우디 A3 모델의 경우 FF 구동 방식을 따른다. 엔진이 앞에 위치하고, 엔진의 동력이 앞바퀴에

전달되는 전륜구동의 형태를 가진다. 따라서 무게중심을 사진과 같이 중심에서 살짝 앞으로 치우친

위치에 설정하였다. 차에서 가장 무게 비중을 많이 차지 하는 것이 엔진이기 때문에 이처럼 설정

하였다. 또한, FF 방식은 앞바퀴로 조향과 구동을 동시에 실시한다.

```csharp
// Steer
        Wheels[0].Left_Wheel.steerAngle = Steering; // 앞바퀴 조향
        Wheels[0].Right_Wheel.steerAngle = Steering;
        // Motor
        Wheels[0].Left_Wheel.motorTorque = Motor; // 앞바퀴 구동
        Wheels[0].Right_Wheel.motorTorque = Motor;
        // Brake
        Wheels[0].Left_Wheel.brakeTorque = Brake;
        Wheels[0].Right_Wheel.brakeTorque = Brake;
```

- **FR (Front engine Rear wheel drive) 방식 : Camaro RS 2015**

---
![image](https://user-images.githubusercontent.com/65384983/206628923-4892d7da-9c09-45f1-ae62-177c8a95b710.png)

해당 모델링이 카마로는 아니지만 가정하고 사용했음

카마로 RS 모델의 경우는 FR 구동 방식을 따른다. FF 와 엔진의 배치는 같지만 후륜구동이라는 점이

다르다. 역시 엔진이 전진 배치이기 때문에, 무게중심을 중심보다 살짝 앞쪽으로 설정하였다.

```csharp
// Steer
        Wheels[0].Left_Wheel.steerAngle = Steering; // 앞바퀴 조향
        Wheels[0].Right_Wheel.steerAngle = Steering;
        // Motor
        Wheels[1].Left_Wheel.motorTorque = Motor; // 뒷바퀴 구동
        Wheels[1].Right_Wheel.motorTorque = Motor;
        // Brake
        Wheels[1].Left_Wheel.brakeTorque = Brake;
        Wheels[1].Right_Wheel.brakeTorque = Brake;
```

후륜구동이기에 뒷바퀴에서 **motorTorque**를 적용한다.

- **RR (Rear engine Rear wheel drive) 방식 : Porsche 911 Carrera**

---

![image](https://user-images.githubusercontent.com/65384983/206628961-4e7801be-e3a8-4254-b015-140dd6479e28.png)

포르쉐911 카레라 모델은 RR 방식을 사용한다. RR 방식은 엔진이 후방에 배치된 후륜구동이다.

따라서 무게중심이 중심에서 조금 더 후방으로 배치되었다. 바퀴의 구동은 FR과 동일한 방식으로

후륜구동을 사용한다.

- **무게중심 설정**

---

```csharp
protected virtual void RigidBodySetUp()
    {
        rigidBody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.FindGameObjectWithTag("CM").gameObject;
        rigidBody.centerOfMass = centerOfMass.transform.localPosi저
```

무게 중심은 **빈 오브젝트**를 각 모델 별 사진의 위치에 설정하고, 해당 오브젝트의 좌표값을

**Rigidbody** 의 **Center Of Mass** 좌표값으로 설정해준다.

### 4. 차량 별 물리 변수 계산

---

각 차량이 실제 차량의 성능과 유사하도록 하기 위해서는 필요한 물리 수치를 계산하여 구해야했다.

차의 특징을 대표하는 지표로 **‘제로백’ (Zero + 100)** 을 사용하였고, 제로백으로 부터 **토크**를 역추적

하여 토크 값을 얻어내서 적용하고, 유니티에서 작동 후 실제 제로백 수치와 근사한 값이 나오는지를

확인하였다.

- **아우디 A3**

---

아우디A3 모델의 제로백은 **약 7초**대라고 한다. 제로백은 시속 100km/h 까지 가속하는데 소요되는

시간이므로, **가속도 공식**에 대입하면 아우디A3의 **가속도**를 구할 수 있다.

$$
a = (V_2 - V_1) / t
$$

먼저 단위를 맞추기 위해 100km/h 를 m/s 으로 환산하면 다음과 같다.

$$
100,000m / 3,600s = 27.77 m/s
$$

이를 가속도 공식에 대입하면, 가속도는 3.967 m/s 이 나온다.

$$
a = (27.77_m/_s - 0_m/_s) / 7_s = 3.967~(m/s)
$$

이 가속도를 통해 아우디A3에 가해지는 **힘(F)**을 구할 수 있다.

$$
F = m * a
$$

$$
F = 1410~(kg : 아우디의~공차중량) * 3.967~(m/s)~=~5593.47~(N)
$$

다시 F 로부터 토크 M 을 구할 수 있다.

$$
M(torque) = r(raidus) * F
$$

아우디 A3의 Wheel Collider의 반지름은 **0.34 unit → 0.34 m** 이다.

따라서, 토크 M 을 구할 수 있다.

$$
M(torque) = 0.34~m * 5593.47~(N) = 1901.78~(kgf)
$$

```csharp
protected override void Init()
    {
        MaxVelocity = 210f; // 아우디A3의 최대 속력
        MaxWheelAngle = 45;
        MaxMotorPower = 1901f; // 계산한 토크를 대입
        MaxBrakePower = 3000f;
    }
```

여기까지 계산해서 얻은 토크를 아우디A3의 MaxMotorPower로 대입한다.

해당 값을 통해 제로백을 테스트해보았다.

Mass 에는 실차 중량을, Drag 에는 금속에

가까운 0.001을 회전저항은 기본값이다.

![image](https://user-images.githubusercontent.com/65384983/206629021-3d4825eb-1f8e-45cb-a77e-6cb63e03e005.png)

---

[https://youtu.be/hox9B4kyHMc](https://youtu.be/hox9B4kyHMc)

약 6.7초가 나왔으며 예상한 제로백 결과값인 7초에 근사했다. 출발 시 ‘GO’ 표시를 보고 출발하기에

사람에 따라, 시도 횟수에 따라 오차범위가 발생하는 것을 감안한다면 실제와 거의 유사한 결과를

얻었다고 생각한다.

뒤에 후술하는 모델도 동일한 과정을 통해 가속도와 토크를 구할 수 있었다.

- **포르쉐 911 카레라**

---

 ****

 **- 제로백 : 4.2초**

 **- 가속도 : 6.611 m/s**

 **- 공차중량 : 1,475 kg**

 **- 타이어 반지름 : 0.35 m**

 **- 토크 : 3412.928 kgf**

 **- 최대 속력 : 291 km/h**

```csharp
protected override void Init()
    {
        MaxVelocity = 293f;
        MaxWheelAngle = 45;
        MaxMotorPower = 3413f; // It means motorTorque
        MaxBrakePower = 5000f; // Brake
    }
```

https://youtu.be/jIcSNJyYP6s

- **카마로 RS**

---

 ****

 **- 제로백 : 4.4초**

 **- 가속도 : 6.311 m/s**

 **- 공차중량 : 1,765 kg**

 **- 타이어 반지름 : 0.38 m**

 **- 토크 : 4232.787 kgf**

 **- 최대 속력 : 288 km/h**

```csharp
protected override void Init()
    {
        MaxVelocity = 288f;
        MaxWheelAngle = 45;
        MaxMotorPower = 4233f; // It means motorTorque
        MaxBrakePower = 6000f; // Brake
    }
```
https://youtu.be/bHgbl0W80YQ

### 5. 속도계 구현 (SpeedoMeter)

---

속도계를 구현하기 위해서는 최대 속도에 따른 현재 속도의 비율이 필요하다.

해당 비는 아래와 같이 구할 수 있었다.

```csharp
speedFactor = Mathf.Abs(rigidBody.velocity.magnitude * 3.6f / MaxVelocity);
```

**Rigidbody**의 **Velocity.magnitude** 는 m/s 의 속도을 반환한다. 따라서 **3.6**의 상수를 곱해

km/h 로 변환한다. 이후, 최대 속도로 나누어 현재 속도비를 구할 수 있다.

속도계는 **큰 눈금 사이의 간격이 45℃** 이다.

따라서 속도계가 표현할 수 있는 **최대 각도는**

**315℃** 다. 따라서, 현재 속도비 만큼 바늘이

움직이면 속도계를 구현할 수 있다. 부드러운

바늘의 움직임을 위해 보간을 사용할 수 있다.

![image](https://user-images.githubusercontent.com/65384983/206629081-c4ec9cbf-45e9-4066-a808-24a9a246af2d.png)

```csharp
rotationAngle = Mathf.Lerp(0, 315, speedFactor);
```

구한 **rotationAngle**을 이용하여 아래와 같이 바늘을 움직인다.

```csharp
arrowPointer.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -rotationAngle));
```

바늘의 회전 방향은 반시계방향이므로 음수로 변환해주고, 오일러각을 쿼터니언으로 변환 대입한다.

![speedo.gif](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/ee7a92fd-5f19-4312-ad7b-f0bb8d6051f4/speedo.gif)

바늘은 속도계의 중앙을 **Pivot**으로 회전한다.

### 6. 로지텍 G29 휠 컨트롤러 세팅

---

소유하고 있는 휠 컨트롤러가 **Logitech G29**모델 하나였으므로, 현재는 하나의 컨트롤러만 지원한다.

**G29**의 연동은 Asset Store 의 **Logitech SDK** 와 Logitech 공식 홈페이지의 Lab 에서 **Wheel SDK** 를

사용하였다.

- **Logitech Gaming SDK : Unity Asset Store**

[Logitech Gaming SDK | Integration | Unity Asset Store](https://assetstore.unity.com/packages/tools/integration/logitech-gaming-sdk-6630)

- **Logitech Wheel SDK : Logitech G Developer Lab**

[Logitech G Developer Lab - Software Development Kits](https://www.logitechg.com/en-eu/innovation/developer-lab.html)

---

- **페달**

---

**G29**의 모든 페달은 **OFF 일 때 32,767**, **ON 일 때 -32,767**의 값을 가진다. **(32,767 ~ -32,767)**

**(클러치, 브레이크, 액셀)**

페달의 **Input Value** 를 Motor 변수에 활용하여 자동차를 구현해야했기에, 

**OFF 일 때 0**, **ON 일 때 Max(Motor, Brake)Power** 가 나올 수 있도록 값의 조정이 필요했다.

이를 위해 적용한 연산은 다음과 같다.

![image](https://user-images.githubusercontent.com/65384983/206629243-adeff6d0-b777-4eb9-8882-d7d52093f78c.png)

해당 과정을 거치면 모든 페달로부터 **0 ~ MaxPower** 사이의 값을 구할 수 있다.

```csharp
Int2Throttle = 65534 / MaxMotorPower; // Convert Int to Throttle pedal value
Int2Brake = 65534 / MaxBrakePower; // Convert Int to Brake pedal value

Motor = Mathf.Round((-controller.lY + Mathf.Abs(controller.lY)) / Int2Throttle); // Throttle
Brake = Mathf.Round((-controller.lRz + Mathf.Abs(controller.lRz)) / Int2Brake); // Brake
```

- **핸들**

---

핸들의 경우, **핸들 회전 값을 핸들의 각도 값**으로 변환하고 **핸들의 각도 값을 바퀴의 각도 값**으로

전환하는 과정이 필요하다.

![image](https://user-images.githubusercontent.com/65384983/206629273-9a5d9663-ab11-4b5e-9825-9e55cac117bc.png)

자동차 바퀴의 최대 조향 각도는 **-45º ~ 45º** 이므로 MaxSteer를 45로 설정하고, G29 휠의 최대

가동범위는 **900º (-450º ~ 450º)** 이므로 MaxHandle 은 450으로 설정하였다.

```csharp
Int2HandleAngle = 32767f / MaxHandleAngle; // 32767 / 450 >> Convert Int to Handle Degree
Handle2WheelAngle = MaxHandleAngle / MaxWheelAngle; // 450 / 45 >> Convert Handle Degree to Wheel Degree

Steering = controller.lX / Int2HandleAngle / Handle2WheelAngle; // Handle
```

- **기어**

---

기어의 조작은 **AUTO 모드**로 **전진, 중립, 후진** 기어를 구현하였다.

**G29** 기어 쉬프터 값은 다음과 같이 가져올 수 있다.

```csharp
for (int i = 0; i < 128; i++) // Gear Button Input
            {
                if (controller.rgbButtons[i] == 128)
                {
                    if (i == 12) // Forward gear D
                    {
                        FrontGear = true;
                        BackGear = false;
                        Debug.Log("1 st Gear Input");
                    }
                    else if (i == 18) // Back gear R
                    {
                        FrontGear = false;
                        BackGear = true;
                        Debug.Log("Backward Gear Input");
                    }
                }
            }

if (controller.rgbButtons[12] != 128 && controller.rgbButtons[18] != 128) // Gear N
            {
                FrontGear = false;
                BackGear = false;
            }
```

12번을 1단 기어, 17번을 6단 기어, 18번을 후진 기어로 **사용가능한 버튼 값**이다.

여기서, **12번을 전진(D)**, **18번을 후진(R)**, 전진도 후진도 아닌 상태를 **중립(N)** 으로 사용하였다.


---

- **키보드 주행 영상**

---

https://youtu.be/9fb-_Vwi5X0

- **휠 주행 영상**
---
https://youtu.be/kImAG1fVpnA
